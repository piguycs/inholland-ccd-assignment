# Assignment submission

The assignment uses Azure Functions for simplicty when deploying and writing
the code. Two queues are used in the project, and two HTTP triggers are used
for the API.

The deployed URL for this project is at: <https://weather-image-app.azurewebsites.net>.

A test file for the live deployment can be found in
[requests.http](./RequestImage/requests.http). However, the access key to the
deployment is not in this file. It will be sent via email to whoever needs
access to it. The `x-functions-key` header is required for accessing the
deployed endpoints, which will be provided to the examiner via email.
