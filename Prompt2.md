## Prompt - Record lighthouses Meteo Statistics 

- Context:
--Our static web app gets all the lighthouses of Argentina's shore with their corresponding Geo coordinates
--Once get then, it shows them geolocalized on a map from Open Street Maps
--Additionally, the app gets the current weather on each lighthouse using Open Meteo API, and shows the weather info on the page,when the user clicks on the lighthouse icon 

- Goal:
- Gather Meteo info through Azure Function triggered by time (1 hour interval) and must include all the lighthouses showed on the Web App 
- Info must contain
-- lighthouse Name`
-- lighthouse coordinates
-- Date
-- Time 
-- Wind Speed (Kms/Hour)
-- Wind Direction
-- Temperature (Celsius)
-- Windchill

- Gathered info must be recorded on an Azure Table contained on an Storage Account
- If an error occurs getting the Meteo info or recording it on the Azure Storage Account Table, info about the error must be recorded on Azure App Insights

-Deliverables:
--An Azure Function App that runs each hour and records the current weather of each lighthouse on an an Azure Storage Account Table
--Deploymen Workflow on Github repo (branch: main) that deploys the Function App everytime, main branch is updated through Pull Request from branc main
--Techinical and architectural documentation of the Function App (with all the azure resoureces involved)

-Azure and Github Technical info
-- Github Repo: 
-- Azure Suscription: 4ffc573f-cffe-48a5-b82a-0f3930ce1700
-- Azure Resource Group: LightHouses_rg
-- Azure Function: LightHousesFunction
-- Azure Storage Account: storagelighthouses
-- Azure Storage Account Key1: NT3eGQtUI3ielsU4VpEyPcGKjq62YwNonX3Tq+3Y52BaaP1PxS9tu87jwhw4ZHvWXitNzejAkdN1+AStyLUGrg==
-- Azure Storage Account Connection String: DefaultEndpointsProtocol=https;AccountName=storagelighthouses;AccountKey=NT3eGQtUI3ielsU4VpEyPcGKjq62YwNonX3Tq+3Y52BaaP1PxS9tu87jwhw4ZHvWXitNzejAkdN1+AStyLUGrg==;EndpointSuffix=core.windows.net
-- Azure Storage Account Table: LightHousesWeather
-- Azure Application Insights: LightHousesFunction
-- Azure Application Insights Connection string: InstrumentationKey=6251c03e-c59f-4b72-bf30-e93bd05f212f;IngestionEndpoint=https://westus3-1.in.applicationinsights.azure.com/;LiveEndpoint=https://westus3.livediagnostics.monitor.azure.com/;ApplicationId=c48eff02-1e41-436f-b8ae-fd175a8779be



