# Introduction

The "23 Video for SharePoint" library provides intuitive web parts for implementing 23 Video into an internal SharePoint based site. The web parts are necessary if you've configured your site to use API authentication for login, in which case the library provides two web parts:

* Inline 23 video - signs a session for your video site and show it in an inline frame (iframe)
* 23 video launch button - signs a session for your video site and displays a button for launching the session

Both web parts are supplied with both source code and project files in the `/src` directory of the repository, and also an installable distribution in the `/dist` directory. For installation instructions please read further down.

All web parts rely on the 23 .NET API implementation, which furthermore relies on the DotNetOpenAuth library. Please refer to the 23 .NET API implementation for more information about how to use the API to extend applications and web parts.

# Installation

To install the web parts into your SharePoint solution, download all files in the `/dist` directory of the repository, and simply run a fitting deployment batch command. If in doubt, use the `deploywithwspbuilder.bat` batch command.

# Using the web parts

Using the web parts in a SharePoint page is straight forward. Simply add the web part you need. After this, you need to configure the web part with your API credentials. In order to use the web parts, you must however have a "privileged API account" set up on your site, and then supply all the credentials to the "Settings" part of the web part configuration.

For the launch button web part you must furthermore supply a label text to the button.