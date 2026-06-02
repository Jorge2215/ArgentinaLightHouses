## Prompt – Record Lighthouses Meteo Statistics

##1 Context
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
-Azure and Github Technical info
-- Github Repo: https://github.com/Jorge2215/ArgentinaLightHouses.git
-- GitHub Repo branch for developlment: dev
-- GitHub Repo branch for deployment: main 
-- Azure Suscription: 4ffc573f-cffe-48a5-b82a-0f3930ce1700
-- Azure Resource Group: LightHouses_rg
-- Azure App Service (Web App): lighthouses-app (Runtime Stack: Dotnetcore - 10.0)
-- App Service Plan: ASP-LightHousesrg-9f88 (Operating System - Linux)
-- Azure Function: LightHousesFunction
-- Azure Storage Account: storagelighthouses
-- Azure Storage Account Key1: NT3eGQtUI3ielsU4VpEyPcGKjq62YwNonX3Tq+3Y52BaaP1PxS9tu87jwhw4ZHvWXitNzejAkdN1+AStyLUGrg==
-- Azure Storage Account Connection String: DefaultEndpointsProtocol=https;AccountName=storagelighthouses;AccountKey=NT3eGQtUI3ielsU4VpEyPcGKjq62YwNonX3Tq+3Y52BaaP1PxS9tu87jwhw4ZHvWXitNzejAkdN1+AStyLUGrg==;EndpointSuffix=core.windows.net
-- Azure Storage Account Table: LightHousesWeather
-- Azure Application Insights: LightHousesFunction
-- Azure Application Insights Connection string: InstrumentationKey=6251c03e-c59f-4b72-bf30-e93bd05f212f;IngestionEndpoint=https://westus3-1.in.applicationinsights.azure.com/;LiveEndpoint=https://westus3.livediagnostics.monitor.azure.com/;ApplicationId=c48eff02-1e41-436f-b8ae-fd175a8779be



