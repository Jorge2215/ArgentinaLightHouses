## Prompt – Record Lighthouses Meteo Statistics
##1. Context
The Static Web App displays all lighthouses along the Argentine coastline with their geo-coordinates.

Each lighthouse is visualized on an OpenStreetMap layer.

Current weather data for each lighthouse is retrieved from the Open Meteo API and shown when the user clicks on the lighthouse icon.

##2. Goal
Implement an Azure Function that:

Runs on a time trigger (every 1 hour).

Collects weather information for all lighthouses displayed in the Web App.

Records the following data fields:

Lighthouse Name

Lighthouse Coordinates

Date

Time

Wind Speed (km/h)

Wind Direction

Temperature (°C)

Windchill

##3. Data Storage & Error Handling
Weather data must be stored in an Azure Table within the designated Storage Account.

If an error occurs (either retrieving Meteo data or writing to the Table), the error details must be logged in Azure Application Insights.

##4. Deliverables
An Azure Function App that executes hourly and records current weather data for each lighthouse into the Azure Storage Table.

A GitHub Actions workflow (branch: main) that deploys the Function App whenever the main branch is updated via Pull Request.

Technical and architectural documentation describing the Function App and all Azure resources involved.

##5. Technical Information
GitHub Repo: [to be provided]

Azure Subscription: 4ffc573f-cffe-48a5-b82a-0f3930ce1700

Resource Group: LightHouses_rg

App Service (Web App): lighthouses-app (Runtime: .NET 10.0, OS: Linux)

App Service Plan: ASP-LightHousesrg-9f88

Azure Function: LightHousesFunction

Storage Account: storagelighthouses

Table: LightHousesWeather

Connection String: [provided securely via secrets]

Application Insights: LightHousesFunction

Connection String: [provided securely via secrets]

