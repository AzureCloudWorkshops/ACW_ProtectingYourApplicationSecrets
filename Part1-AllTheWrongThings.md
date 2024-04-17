# Part 1 - All The Wrong things

After getting the resources deployed using the bicep templates, it's time to create the project with all the wrong things and the push it into Azure.

## Task 1 - Get the code

The starter and final versions of the code can be found in the `Resources` folder of this repository.

You can either fork this repository, or just download the repository as a zip file and extract it to a folder on your machine.  If you fork the repository, you will need to clone it to your machine.  If you download, you will need to create your own repo to push the code to at GitHub for publishing to Azure App Service.  

### Fork Option

1. [Fork the Repository](https://docs.github.com/en/get-started/quickstart/fork-a-repo)
1. [Clone the repository to your machine](https://docs.github.com/en/get-started/quickstart/fork-a-repo#cloning-your-forked-repository)

### Download Option

1. [Download the repository code](https://sites.northwestern.edu/researchcomputing/resources/downloading-from-github/)
1. [Create a new GitHub Repository](https://docs.github.com/en/get-started/quickstart/create-a-repo)  
1. [Push the starter code to the repository](https://gist.github.com/c0ldlimit/4089101) 

## Task 2 - Ensure the code is working

After downloading/forking/etc the code, open the solution in Visual Studio locally and run the application.  

Run `update-database` in the PMC or Navigate to the link to migrate the database, which should be something like:

```https:
https://localhost:7213/home/migratedatabase
```  

Ensure you can register a user and log in, which you will need to do to be able to see the images (it is assumed you have enough experience to complete this task with minimal guidance).

![](images/Part1/image0000-homepagelocalaftermigrationandregisteruser.png)

>**Note:** you will not see images until you configure your repo secrets for the storage account.

1. Push your code to your repository

Make sure the initial files are in your repo for deployment and settings updates (once again, it's assumed you can get code to your repo with minimal guidance).

![Initial GitHub Files](images/Part1/image0000-repoafterinitialfilepush.png)  

## Task 3 - Publish

With your code in your repo, publish it to Azure App Service using the following cloud workshop as a guide to set up CI/CD:

[Deploy App Service to Azure via GitHub Actions](https://github.com/AzureCloudWorkshops/ACW_DeployAppServiceToAzureViaGitHubActions)  

## Task 4 - Initial setup completion check.

Before moving on, ensure that you have the following:
- A GitHub repository with the code
- A published Azure App Service that is displaying the website but you can't register or log in because no database information is configured.  Even if you could you have not configured the storage account settings yet so that part would not be useful.

![Deployed Web](images/Part1/image0001-deployedweb.png)  

## Task 5 - Enable Repository Security tools

One of the most important things you can do to protect your secrets is to enable repository security tools.

Before creating problems, you should enable the tools to alert you to potential secret leaks.

>**Note:** For this walkthrough you will be using your OWN repository.  For that reason, do not be alarmed at the name(s) of the repository(ies) in the screenshots.  You will be using your own repository for this walkthrough, and yours will also be different.  

### Step 1 - Turn on GitHub Security

Navigate to your repository in GitHub and click on the Security tab.

![](images/Part1/image0002-githubsecurity.png)  

1. Turn on Dependabot alerts (optional - recommended for SPA frameworks)

Dependabot alerts will let you know if any of the packages you have in your project are out of date.  This is not as critical for an MVC site but is very important if you are using a SPA framework like React or Angular.

![](images/Part1/image0003-dependabotalerts.png)  


1. Get GitGuardian (free or premium versions available)++

Sign up for the free version of GitGuardian to see how to work with it in your repositories [Git Guardian](https://dashboard.gitguardian.com/auth/signup?utm_source=website&utm_medium=product&utm_campaign=gim_desc_page)  

Once you've signed up, you'll be able to see your violations on a dashboard at GitGuardian.  You will also receive email notifications when a violation is detected.

![](images/Part1/image0004-gitguardian.png)

++For a team at work you will need to upgrade to a paid version.

1. Add the provider to your GitGuardian account (if it's not already integrated)

You will need to add the repository organization (i.e. GitHub -> yourusername) to allow it to be monitored.

Here you can see my personal GitHub is connected but options exist to add other providers like ADO, GitLab, GitHub Enterprise and BitBucket.

![](images/Part1/image0005-gitguardianproviders.png) 

If you want to go deeper, you can also create custom integrations for notifications:

![](images/Part1/image0006-gitguardianintegrations.png)  

## Task 2 - Set the secrets locally

Since this is all the wrong things, you will start by putting the secrets directly in `appsettings.json`, then checking them in so they get committed to GitHub.  

You will need to set the following secrets in `appsettings.json`:

- Connection String for SQL Db
- Storage account Connection String (used to pull the images)
- ApplicationInsights Connection String (optional, will be used later, may as well get it now)

>**Note:** For brevity, this website doesn't use the storage account connection string and the SDK to get container images, but instead just uses a container SAS token (which expires and won't be valid by the time you're reading this if the account even still exists).  In other solutions you might encounter the SDK with the Storage Account Connection string instead of SAS tokens (especially for uploads).  The storage account connection string must be rotated to ensure that the secret is kept safe.  Either way, the connection string or SAS token should be protected, and the idea is the same in spirit - don't put your secret where it can be compromised.  

1. Set the Database Connection String

Find the database connection string in Azure under the `Connection Strings` navigation for the SQL Database.  Copy the connection string and paste it into the `appsettings.json` file.  Replace the `{your_password}` with the password you set when you deployed the database from the `iac` template.  

![](images/Part1/image0007-dbconnectionstrings.png)  

If you don't remember the password, use the portal to reset the password and get your updated password for the connection string.  For example:

```text
Server=tcp:protectyoursecretsdbserver20251231acw.database.windows.net,1433;Initial Catalog=ProtectYourSecretsDB;Persist Security Info=False;User ID=secretweb_user;Password=Azure#54321!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```  

>**note:** remember to change the password or delete the database after you are done with this walkthrough.  Adding the password into the repository is a high risk security breach, and it should be considered compromised once committed and treated as such.

![](images/Part1/image0008-connectionstringhardcoded.png)  

1. Create a SAS Token for the storage account and set it in `appsettings.json`

On the storage account page, go to the `images` container and create a SAS token with read permission (set expiration appropriately, default is 8 hours):

![](images/Part1/image0009-containersastoken.png)  

Create the token, then copy the SAS **token** (not the full url!!) and paste it into the `appsettings.json` file.  Replace the `ImagesSASToken` value with the SAS token you just created.

![](images/Part1/image0010-sastokentoappsettings.json.png)  

>**Important:** You must also update the name of your storage account to the correct name of your storage account generated during the deployment.  If you fail to do this you will not get the images from the storage account container correctly.

1. Optional but recommended (you'll need it later anyway) - Get the full connection string for the App Configuration and put it into `appsettings.json`. 

>**Note:** this will not be used until later in the walkthrough, but if you get it now you can move it along with other secrets and be ready to use it when required.

![](images/Part1/image0011-appconfigconnectionstring.png)   

Your connection string should be something like (with a different secret key of course):

```text
Endpoint=https://ac-protectingyoursecrets-20251231acw.azconfig.io;Id=co+R;Secret=4IDve......lHM=
```

Place the connection string in the `appsettings.json` file.  Replace the `TBD` in the `ConnectionStrings:AzureAppConfigConnection` with the connection string you just copied.

>**Note:** if you are not destroying resources after this activity, make sure to regenerate the secret key after you are done with this walkthrough.  The secret key should be considered compromised once it is committed to the repository.  Do the same for the storage key as well.

![](images/Part1/image0011-appconfigconnectionstring.png)  

Get the connection string and then put it into `appsettings.json` if you so desire.

![Compromised Settings](images/Part1/image0010-verybadcompromisedsecretsinappsettings.json.png)  

## Task 3 - Push the changes to Github

To make this all the wrong things, you will need to push the changes to GitHub so that the secrets are stored in the repository.

While this **should** allow the app to work you will now have exposed your secrets in the repo.  This is a bad thing because any code in GitHub should be considered compromised.  If you have a public repo, this is even worse because anyone can immediately see your secrets, as well as fork/clone your code and have them permanently.  They can then look at your other repositories and if you have done this in multiple places they can find your patterns, such as using the `Azure#54321!` password for all of your test databases.

1. Commit the changes to GitHub

Commit and push to GitHub.  Wait for the app service to deploy.

![](images/Part1/image0013-commitandpushtogithub.png)  

1. While the website deploys, review your GitGuardian website.

GitGuardian has already detected that I published some secrets. 

![](images/Part1/image0014-gitguardiansecretsdetected.png)  

Additionally, I've been emailed about the egregious error on my part:

![](images/Part1/image0015-gitguardianemail.png)  

1. Check the website

With the website deployed, navigate to the public URL for the website and attempt to Register.

![](images/Part1/image0016-errorfornomigrations.png)

1. Check the database firewall

It's always a good idea to make sure the services can communicate.  On the server firewall (Networking) make sure to enable `Selected Networks` then check the box for the `Allow Azure services and resources to access this server` option.  Additionally, add your local IP to the firewall so you can run the migrations from your dev machine or query the database if necessary (Clearly this is **not** a production solution for migrations).

![](images/Part1/image0017-allowazureandyourmachinetohitthedatabase.png)  

1. Run the migrations by clicking on the link

With the secrets hardcoded, you can do all the things, which is nice, but insecure.  You should be able to migrate the database just by hitting the link `Migrate Database`

1. Register the user

Confirm the email or you won't be able to log in:

![](images/Part1/image0018-confirmemail.png)  

Hit the `X` on the `Thank you for Confirming` page.

1. Login as the user

Once you've registered the user, you should be able to log in and see the images.  If you can't see the images, you may need to run the migrations manually.

Again, this is great, but bad because you've hardcoded your secrets into the repository.  This is a bad practice and should be avoided at all costs.

![You can see the data!](images/Part1/image0016.5-hardcodedsecretsworkbutareverybad.png)  


## Task 4 - Decomprosmise the secrets

Because you hard coded them, now would be a great time to regenerate the keys and then validate the site is broken again.

1. Rotate the connection string for the app config.  Make a note of the new connection string, but do not put it in the `appsettings.json` file.

![Rotate App Config Connection String](images/Part1/image0020-rotateappconfigconnectionstring.png)  

1. Change the database user password:

Navigate to the server and change the password (Make sure to make a note of the new password but do not put it in the appsettings.json file).  

![Change the Database Password](images/Part1/image0021-changedbpassword.png)  

1. Rotate the storage account key(s)

Navigate to the storage account and rotate the keys.  You will need to create a new SAS token after doing this to get the images to display again.  Make a note of the new sas token but don't put it in the `appsettings.json` file.

![Rotate the Storage Account Key](images/Part1/image0022-rotatestoragekeys.png)   

1. Navigate to the site.  It should not work again if all keys were successfully rotated.

You will get it working again in the next steps, but for now, it should be broken.  You will see an error until you log out, then you will go back to the default view and won't be able to log in.  This is expected.

## Completion Check

Once everything was working and you've rotated keys (optionally/recommended), you are ready to move to the next step.
