# EmailBroadcast
Send a mail to a group with one click
## Building and Running
You can build using Visual Studio or Docker. <br />
The website has been developed using ASP.NET Core and React. Firestore database has been used.

Before running, set the following in environment variables:
- GOOGLE_APPLICATION_CREDENTIALS = Path to your Firebase Admin SDK service account file.
- SenderEmail = Your sender email.
- EmailApiKey = Send In Blue API key.
- FirebaseProjectId = Firebase project id.

To build using docker
```
docker build -t email-broadcast .
```
After that run by
```
docker run -it -p 6000:6000 -e PORT=6000 email-broadcast
```
Then go to http://localhost:6000
