# Part 2 - Securing Secrets

Before starting this part, ensure that you have completed [Part 1 - All the wrong things](Part1-AllTheWrongThings.md), and that you have a working website that integrates secrets for the database connection string and the storage account SAS token.

## Task 1 - Move all of the secrets out of `appsettings.json` into local user secrets `secrets.json`

To get started, the first tool you want to leverage is the `local user secrets` option available in Visual Studio.  This will allow you to store secrets in a local file that is not checked into source control, and is only available on your local machine.  

This is a great option for development, but not for production.  

Additionally, this is not necessarily the `best` option for a development team, as every developer has to have their own copy of `secrets.json` on their machine. For an entire team, you will likely prefer to create a shared vault with developer secrets and just point to that vault for any secrets that are needed rather than try to have every developer set up local secrets.

1. Right-click and select `Manage User Secrets`

![](images/Part2/image0001-manageusersecrets.png)  

1. Use the sample file included to copy and paste the key-value pairs into the `secrets.json` file.  

This will ensure that you have the correct keys and then you can just update values from the `appsettings.json` file. 

![](images/Part2/image0002-usersecrets.png)  

1. Update the values with the values from the `appsettings.json` file.

Copy and paste the values so they are all updated in the `secrets.json` file.

![](images/Part2/image0003-secretsjsonupdated.png)  

1. With the secrets set in the `secrets.json` file, remove the sensitive information from `appsettings.json` and save the file.

>**Note:** you can leave the local db setting if you want, just don't override it in the `secrets.json` if you leave it in `appsettings.json` and make sure it's NOT set to the Azure SQL database connection string.

![](images/Part2/image0004-appsettingsjsonupdated.png)  

>**Note**: You could move the app settings for the storge details into secrets if desired.  If you do that, you would need to add configuration values at Azure in the next steps to map to every key-value pair in the `secrets.json` file.

1. Run the app locally and make sure the database and storage still work.

Ensure that your application still runs (you may need to run migrations locally if you didn't already do that - and register a user).  Once you are certain the changes work locally, push to GitHub.

>**Note:** you should not have to make any code changes for this to work, just move the secrets to the new file.

![](images/Part2/image0005-secretsoutofgithub.png)  

This push will re-deploy the application, and now it won't work.  Why do you think that is?

The database connection string and the SAS token are no longer available.

## Completion Check

At the end of this first task, you should have removed all of the secrets from your local appsettings.json file and moved them to the secrets.json file.  You should also have a working website locally that is using the secrets.json file.

The website is no longer working at Azure and will be fixed in the next task.

## Task 2 - Put the raw secrets in Azure App Service Configuration

To get the site working, put the database connection string and SAS token into the Environment and Configuration variables in the Azure App Service.

1. Add the database connection string.

In your app service, under `Configuration` then under `Connection Strings` add the connection string for the database.

- Name: `DefaultConnection`
- Value: `your-connection-string`
- Type: `SQLAzure`

![](images/Part2/image0006-databaseconnectionstring.png)  

>**Note:** make sure to save everything, which is a triple `OK` process.   
1) Hit Ok  
2) Hit Save a the top of the screen
3) Hit Continue to save it.

The use of the `deployment slot` is optional and only matters if you are using slots.  I check it because it reminds me that I set it, and prepares me to have different values available for test slots in the future as this will be sticky to the `production` slot.  The Free app service doesn't have slots so it's absolutely irrelevant for this walkthrough.

1. Add the SAS token value for the storage account in the Environment Variables (Application Settings).

![](images/Part2/image0007-storagesastokenenvironmentvariable.png)  

>**Note:** Make sure to do the triple save thing again.

1. Add the App Configuration Connection String

Once again this is not yet in use but it's great to just get it in here for now.  This will be updated in part 3.

![](images/Part2/image0008-azureappconfigsecret.png)  

1. Notice the Application Insights

Make a note that the Application Insights settings are already in place.

![](images/Part2/image0010-settingscompleted.png)  

1. Test the website

With the configuration settings and database connection in place, the website should once again be working

## Completion Check

At the end of this part, you should have no secrets in your GitHub repository, and the website should be working based on the use of secrets.json on the local machine and the configuration settings in the Azure App Service.

## Task 3- Move the secrets to Azure Key Vault

Now that the secrets are mitigated from code, it's time to hide them from people who don't need to see them.

To securely store your secrets, you'll need to complete a couple of tasks.  First you will put the secrets into KeyVault.  Then you will change the settings to leverage KeyVault for secrets, and finally you will create a managed identity for the app and authorize that identity in the Azure Key Vault Policy.

1. Move the secrets to Azure Key vault.

To get started, open your Key Vault and add two secrets.  The `name` of the secret doesn't matter if you aren't directly leveraging it in code, but the name will be part of the URI so you will want to keep it short and meaningful.

![](images/Part2/image0011-createanewkvsecret.png)  

>**Note:** You can leverage secrets directly from code. This walkthrough will not take that approach however.  If you are interested in retrieving secrets via code, you can review [this learn tutorial](https://learn.microsoft.com/en-us/azure/key-vault/general/tutorial-net-create-vault-azure-web-app?WT.mc_id=AZ-MVP-5004334).  

If you currently see a note that says you are `unauthorized` and `the operation "List" is not enabled...` then you need to add your identity to the KeyVault policies.

![](images/Part2/image0011.1-keyvaultaccessdenied.png)  

Click on `Access Policies` and then `Create`.  

![](images/Part2/image0011.2-keyvaultaccesspolicies.png)

Select all the permissions for Keys, Secrets, and Certificates, then hit `Next`.

![](images/Part2/image0011.3-keyvaultaccesspoliciespermissions.png)

Select your principal identity and hit `Next`:

![](images/Part2/image0011.4-keyvaultaccesspoliciesprincipal.png)

Hit `Next` to skip the `Application (optional)` tab.

Click `Create` on the `Review + Create` tab to save the policy.

![](images/Part2/image0011.5-keyvaultaccesspoliciesadd.png)

For simplicity, name the first secret something like

```text
ProtSecDbConnectionString
```  

Set the value to the connection string for the database.

Optionally, you can set the content type to `string` and you can alter the active dates or even set the secret to `disabled`.  You can also add metadata tags if you wish.

![](images/Part2/image0012-createsecretdbconnection.png)  

Repeat the process for the storage account SAS token.

```text
ProtSecStorageSASToken
```  

For both secrets, check the value by drilling into them and hitting the `Show Secret Value` button.

![](images/Part2/image0013-showsecretandgeturi.png)  

While on each secret, copy the `Secret Identifier` URI.  You will need this in the next step for BOTH secrets. So keep them in a notepad.

The values should look similar to the following:

```text
https://kv-protsec-20251231acw.vault.azure.net/secrets/ProtSecStorageSASToken/7f6e8703a0504f0889e04545a73c7ab1

https://kv-protsec-20251231acw.vault.azure.net/secrets/ProtSecDbConnectionString/03df3ab51a5f44168f95276f9e8efadb
```  

Remove the version number from the end so that all you get is the raw URL to the name of the secret.  By removing the version number, you'll always get the latest version of the secret:

```text
https://kv-protsec-20251231acw.vault.azure.net/secrets/ProtSecStorageSASToken

https://kv-protsec-20251231acw.vault.azure.net/secrets/ProtSecDbConnectionString
```

1. Update the configuration settings in the Azure App Service.

Now that the secrets are in Key Vault, you can update the configuration settings in the Azure App Service to leverage the secrets from Key Vault.  

To do this, you will need to update the configuration settings to use the `@Microsoft.KeyVault(SecretUri=...)` syntax to wrap the URI to the secret in Key Vault.

The values should look similar to the following:

```text
@Microsoft.KeyVault(SecretUri=https://kv-protsec-20251231acw.vault.azure.net/secrets/ProtSecStorageSASToken)
``` 
![](images/part2/image0014-sastokenfromvault.png)  

And

```text
@Microsoft.KeyVault(SecretUri=https://kv-protsec-20251231acw.vault.azure.net/secrets/ProtSecDbConnectionString)
```  

![](images/Part2/image0015-dbconnectionfromvault.png)  

Once you have updated both, you'll get a notification that they are `Key Vault References` and you will see that they are not working.

![](images/Part2/image0016-kvrefnotworking.png)  

The reason they are not working is because the application is not authorized to read from Key Vault.

![](images/Part2/image0017-kvaccessdenied.png)  

1. Create a Managed Identity for the App Service  

Click on the `Identity` tab on the left navigation of the App Service.  When the window comes up, give the app service a system managed identity (or validate that one is already assigned).

>**Note:** The iac for the app service assigns a system managed identity for you.

![](images/Part2/image0018-systemassignedmanagedidentity.png)

Copy the `Object (principal) ID`  You will use this to authorize the app service to read from Key Vault.

1. Return to the Key Vault and add permission to `Get` secrets from the Key Vault.

Under `Access Policies` click `Create` and then select `Get` under  `Secret Permissions` from the dropdown.  You do not need to select any other permissions (and you don't want to).  Thinking least privilege, you don't want the app to be able to iterate all the secrets and get them all, so the only thing it can do is get the secrets which is specifically knows about.

![](images/Part2/image0019-getsecretsfortheapp.png)  

Hit `Next`

1. Paste the object principal ID into the `Search` box and select the app service principal.

![](images/Part2/image0020-selectappserviceprincipal.png)  

Hit `Next`.

The `Application (optional)` will say that an application is already selected, so hit `Next` 

![](images/Part2/image0021-alreadyappselected.png)  

On the `Review + Create` page, hit `Create` to save the policy.

![](images/Part2/image0022-createappgetsecretpermission.png)  

Ensure your application is listed with `Get` Secret permissions.

![](images/Part2/image0023-apphasgetpermission.png)  

1. Restart the web application to allow it to fetch the settings from Key Vault.

![](images/Part2/image0024-restartwebapp.png)  

>**Note:** it may take up to five to ten minutes for the application to register that it has permissions for the secrets, if it's not working, get a cup of coffee and restart again.  

![](images/Part2/image0025-keyvaultreferenceworking.png)  

1. With the application showing that it has permission, view the website and validate that the secrets retrieved from Key Vault are working as expected.

## Completion Check

You've reached the end of part 2.  In this section you learned how to remove secrets from app settings into your local user secrets, and then you saw how to put the secrets into the app configuration settings at Azure.  From there, you moved the secrets into Azure Key Vault and no longer have any direct secret information in the app service or your application code that is checked into GitHub.

With this, you've now made it possible for secret values like connection strings to be completely unknown to anyone but Key Vault admins for your production and development apps at Azure.
