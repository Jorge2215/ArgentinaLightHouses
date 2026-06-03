### 2026-06-02: WeatherGrid table dark theme fix
**By:** Jorgito (requested)
**What:** Bootstrap .table sets white backgrounds on tbody/tr/td that override transparent. Always set explicit background-color on alh-table and its children rather than relying on transparent inheritance.
**Why:** Low contrast between light-blue text and white Bootstrap default background made data unreadable.
