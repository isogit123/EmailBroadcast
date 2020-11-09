# EmailBroadcast
Send a mail to a group with one click
## Building and Running
You can build using Visual Studio or Docker.

Before running, add the following in Emails/appsettings.json:
- Your database connection string in *context* field in line 11.
- Your email service api key in *EmailApiKey* field in line 13.
- Sender email in *SenderEmail* field in line 14.

To build using docker
```
docker build -t email-broadcast .
```
After that run by
```
docker run -it -p 6000:6000 -e PORT=6000 email-broadcast
```
Then go to http://localhost:6000

There is a live demo of the website on https://email-obscura.herokuapp.com/
