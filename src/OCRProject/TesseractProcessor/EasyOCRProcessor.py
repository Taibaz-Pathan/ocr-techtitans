import easyocr
import sys
import json

def extract_text(image_path):
    reader = easyocr.Reader(['en'])  # Initialize OCR reader for English
    results = reader.readtext(image_path)

    extracted_text = "\n".join([text for _, text, _ in results])
    print(json.dumps({"text": extracted_text}))  # Return JSON format

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print(json.dumps({"error": "No image path provided"}))
    else:
        extract_text(sys.argv[1])  # Read image path from command-line argument
