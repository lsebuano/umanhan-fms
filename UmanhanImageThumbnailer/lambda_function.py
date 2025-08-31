import json
import os
import logging
from io import BytesIO

import boto3
from PIL import Image

# Configure logging
logger = logging.getLogger()
logger.setLevel(logging.INFO)

# Initialize S3 client (uses IAM role or env vars)
s3 = boto3.client('s3')

# Thumbnail size
def create_thumbnail(image_bytes, size=(100, 100)):
    with Image.open(BytesIO(image_bytes)) as img:
        #img.thumbnail(size, Image.ANTIALIAS)
        img.thumbnail(size, Image.Resampling.LANCZOS)
        out_buffer = BytesIO()
        # Save as JPEG (convert if needed)
        if img.mode in ('RGBA', 'LA'):
            # Convert transparency to white background
            background = Image.new('RGB', img.size, (255, 255, 255))
            background.paste(img, mask=img.split()[3])
            background.save(out_buffer, 'JPEG', quality=75)
        else:
            img.convert('RGB').save(out_buffer, 'JPEG', quality=75)
        out_buffer.seek(0)
        return out_buffer


def lambda_handler(event, context):
    # Process each record in the event
    for record in event.get('Records', []):
        bucket = record['s3']['bucket']['name']
        key = record['s3']['object']['key']
        logger.info(f"Processing file {key} from bucket {bucket}")

        # Download original image
        try:
            response = s3.get_object(Bucket=bucket, Key=key)
            original_bytes = response['Body'].read()
        except Exception as e:
            logger.error(f"Error downloading {key} from bucket {bucket}: {e}")
            continue

        # Create thumbnail
        try:
            thumb_buffer = create_thumbnail(original_bytes)
        except Exception as e:
            logger.error(f"Error creating thumbnail for {key}: {e}")
            continue

        # Build thumbnail key
        thumb_key = f"thumbnails/{key}"

        # Upload thumbnail
        try:
            s3.put_object(
                Bucket=bucket,
                Key=thumb_key,
                Body=thumb_buffer,
                ContentType='image/jpeg'
            )
            logger.info(f"Thumbnail uploaded to {thumb_key}")
        except Exception as e:
            logger.error(f"Error uploading thumbnail {thumb_key}: {e}")

    return {
        'statusCode': 200,
        'body': json.dumps('Thumbnail generation completed')
    }
