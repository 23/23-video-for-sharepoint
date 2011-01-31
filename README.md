# Introduction

The "23 Video for SharePoint" library provides intuitive web parts for implementing 23 Video into an internal SharePoint based site. The web parts are necessary if you've configured your site to use API authentication for login, in which case the library provides two web parts:

* 23 Video inline - signs a session for your video site and show it in an inline frame (iframe)
* 23 Video launch button - signs a session for your video site and displays a button for launching the session
* 23 Video embed - embeds a selected video from your video site
* 23 Video list - shows a list of videos from your video site based on channel and tags

All web parts are supplied with both source code and project files in the `/src` directory of the repository, and also an installable distribution in the `/dist` directory. For installation instructions please read further down.

All web parts rely on the 23 .NET API implementation version 1.1, which furthermore relies on the DotNetOpenAuth library. Please refer to the 23 .NET API implementation for more information about how to use the API to extend applications and web parts.

# Installation

1. Install the web parts into your SharePoint solution by downloading all files in the `/dist` directory of the repository, and simply running a fitting deployment batch command. If in doubt, use the `deploywithwspbuilder.bat` batch command.
2. Create a privileged API account on your video site and save the credentials
3. Append the following section to your web.config for the SharePoint solution (insert your domain and credentials where shown in brackets):

      <appSettings>
        <add key="TwentythreeDomain" value="[domain of your video site]" />
        <add key="TwentythreeConsumerKey" value="[API consumer key]" />
        <add key="TwentythreeConsumerSecret" value="[API consumer secret]" />
        <add key="TwentythreeAccessToken" value="[API access token]" />
        <add key="TwentythreeAccessTokenSecret" value="[API access token secret]" />
    </appSettings>

4. Go to the Site Collection Features section and enable the 23 Video web parts that you want

# Using the web parts

Using the web parts in a SharePoint page is straight forward. Simply add the web part you need. After this, each of the web parts feature a range of settings that can be applied.