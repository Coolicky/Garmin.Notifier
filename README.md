# Garmin Activity Notifier

Quick app to send notifications when you start activity with Garmin Live Track.
As your loved ones (just like mine) probably don't check email i wanted something to send them push notification on their phone.

It will monitor your desired email for Garmin messages and will send a notification with the link when the email is received.

Currently works with Home Assistant notifications but it could be easily expanded to add more if needed.

## Setup

Copy `docker-compose.yml` file from `/deploy` and update configuration

```yml
services:
  garmin.notifier:
    image: coolicky/garmin.notifier
    environment:
      #Imap Settings
      - Imap__Host=imap.domain.com #Replace with your IMAP server address
      - Imap__Port=993 #Default is 993 
      - Imap__UseSsl=true #Default is true
      - Imap__Username=USERNAME #Replace with your email address
      - Imap__Password=PASS #Replace with your email password
      #HomeAssistant Settings
      - HomeAssistant__EntityIds=entity_id_1,entity_id_2 #Replace with your Home Assistant entity IDs (basically the mobile devices)
      - HomeAssistant__Message=MESSAGE
      - HomeAssistant__Title=TITLE
      - HomeAssistant__Url=https://home.domain.com #Replace with your Home Assistant URL
      - HomeAssistant__Token=TOKEN #Replace with your Home Assistant long-lived access token
```
