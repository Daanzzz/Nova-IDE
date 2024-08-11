from email.mime.image import MIMEImage
import smtplib
import os
import base64 
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from argparse import ArgumentParser


parser = ArgumentParser(description="Send an email with embedded random number")
parser.add_argument('--recipient', required=True, help="Recipient's email address")
parser.add_argument('--random_number', type=int, required=True, help="Random number to include in the email")
args = parser.parse_args()

#random_integer = random.randint(100000, 999999)
random_integer = args.random_number

msg = MIMEMultipart()

# Email content
html_content = """
<!DOCTYPE html>
<html>
<body>
<h1>Verification Code</h1>
<img src="cid:image1">
<head>
<style>
  /* Add CSS styling to make the text bold, bigger, and left-aligned */
  p {
    font-weight: bold;
    font-size: 24px; /* You can adjust the font size as needed */
    text-align: left;
  }
</style>
</head>
<body>

<p>""" + str(random_integer) + """</p>
</body>
</body>
</html>
"""


msg.attach(MIMEText(html_content, 'html'))

image_path = 'NovaFull.png' 
with open(image_path, 'rb') as image_file:
    image_data = image_file.read()
    image_base64 = base64.b64encode(image_data).decode('utf-8')

image = MIMEImage(image_data)
image.add_header('Content-ID', '<image1>')
msg.attach(image)

recipient = 'receiving_mail'
recipient = args.recipient
sender_email = 'your_sender_mail'

msg['Subject'] = 'Nova Email Verification'

msg['From'] = sender_email
msg['To'] = recipient

try:
    mail = smtplib.SMTP('your.mail', 000)
    mail.ehlo()
    mail.starttls()
    
    mail.login('your_mail', 'your_mail_pass')

    mail.sendmail(sender_email, recipient, msg.as_string())

    mail.close()

    print("Email sent successfully!")
except Exception as e:
    print(f"An error occurred: {e}")