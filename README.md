# Step Function Example

Here on this repository we have an example of Step Function which was designed to migrate a legacy system into a new structure and one Lambda showing how we handled execution timeout during the flow. 

In order to make it run, you have to follow some steps. 

## Deploying Migration Lambda Function:

1 - Setup [AWS Cli](https://aws.amazon.com/cli/) in your machine.

2 - Create a deployment S3 bucket in your AWS environment.

3 - Create a new role giving `LambdaFullAccess` and `AWSLambdaBasicExecutionRole` to attach it on Migration Lambda during deployment.

4 - `cd src/MigrationLambda`

5 - Run `dotnet lambda deploy-function` to deploy the Lambda

## Creating Step Function:

1 - Reach to [AWS Step Function](https://us-east-2.console.aws.amazon.com/states/home?region=us-east-2#/homepage) and create a new Hello World one.

2 - Attach `LambdaFullAccess` to create Step Function IAM.

3 - Copy `migration-definition.json` and replace what you have on Step Function definition in AWS.

4 - Update MigrationLambda ARN on Step Function definition based on what you have on your environment.

Now, you'll be able to run it through AWS Console.

## Additional information

You can also setup an API Gateway to trigger Step Function from outside AWS Console. You can check how to do it [here](https://docs.aws.amazon.com/step-functions/latest/dg/tutorial-api-gateway.html).