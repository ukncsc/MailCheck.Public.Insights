# MailCheck.Insights
The Mail Check Insights Microservice is responsible for extracting key information from enriched aggregate report records in the form of a DMARC Insights report which is useful to the user to help them understand the impact of setting up their email security.
You should have a database connection established with the Insights DB when running any project which contains a DAO folder.
## Environment Variables needed for running:
In general, when running any project within MailCheck.Insights you should have the following environment variables set up:
|Variable  | Value |
|--|--|
| SnsTopicArn | arn of topic to publish messages  |
| MicroserviceOutputSnsTopicArn |arn of topic for api to publish messages
| ConnectionString |database connection string | 
| DevMode | boolean to toggle CORS and run on localhost | 
| NameServer | NameServer used in the Poller
| AWS_REGION | aws datacentre region  |
| AWS_ACCESS_KEY_ID |aws access key  |
| AWS_SECRET_ACCESS_KEY |aws secret access key  |
