## Prompt 3 - Add Weather Data Grid Page

1. Context
The LightHousesWeather table contains hourly weather statistics for Argentine lighthouses (Name, Coordinates, Date, Time, Temperature, Wind Speed, Wind Direction, Windchill).

The current Web App shows lighthouses on a map with real-time weather info when clicking a marker.

2. Goal
Create a new grid view page accessible from the Home Page that:
Displays the weather records from the LightHousesWeather table.
Allows filtering (e.g., by Lighthouse Name).
Allows sorting ascending/descending by any column (Temperature, Wind Speed, Date, etc.).
Provides pagination for large datasets.
Maintains a design consistent with the Home Page.
Includes a title: Argentina Lighthouses Weather.
Shows a lighthouse image in the top-right corner of the page.

3. Requirements
Add a link on the Home Page to navigate to the new grid page.
Implement filtering and sorting (client-side or server-side).
Ensure responsive design (desktop and mobile).
Optimize the lighthouse image for web performance.

4. Deliverables
A new grid view page integrated into the Web App.
Updated Home Page with navigation link.
Technical documentation describing:
How the grid retrieves data from Azure Storage Table.
How filtering and sorting are implemented.
Any libraries/frameworks used (e.g., React Data Grid, Bootstrap Table).

5. Technical Notes
Data source: LightHousesWeather table in storagelighthouses Storage Account.
Expose table data securely via Azure Functions or API endpoints.
Do not expose connection strings in frontend code; use environment variables or secrets.
Image asset: store in the Web App’s static folder or a CDN.

6. Wireframe textual (mockup)
------------------------------------------------------------
| Argentina Lighthouses Weather              [ Lighthouse ] |
------------------------------------------------------------
| Filter: [ Lighthouse Name ▼ ]  Sort: [ Temperature ▼ ]    |
------------------------------------------------------------
| Name            | Date     | Time  | Temp | Wind | Chill  |
|-----------------|----------|-------|------|------|--------|
| Faro Año Nuevo  | 02-06-26 | 22:00 | 7.1°C| 40kmh| 0.2°C  |
| Faro Almirante  | 02-06-26 | 23:00 |14.5°C| 29kmh|11.2°C  |
| Faro Esperanza  | 02-06-26 | 22:00 |-9.6°C| 14kmh|-15.2°C |
| ...             | ...      | ...   | ...  | ...  | ...    |
------------------------------------------------------------
| [ Pagination: 1 2 3 ... ]                                  |
------------------------------------------------------------
