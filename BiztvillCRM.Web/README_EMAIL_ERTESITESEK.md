# Email Értesítések - Azure Free Tier megoldás

## Probléma
Az Azure Free Tier App Service inaktivitás után alvó módba kerül, így a háttérszolgáltatás (BackgroundService) nem fut.

## Megoldási lehetőségek

### 1. **Beépített admin beállítás** (Implementálva)
- Az admin beállíthatja a futási időt az SMTP beállításoknál
- Alapértelmezett: 06:00
- ±10 perc tolerancia

### 2. **UptimeRobot** (Ajánlott ingyenes megoldás)
1. Regisztrálj: https://uptimerobot.com (ingyenes)
2. Hozz létre egy új HTTP(S) monitort
3. URL: `https://your-app.azurewebsites.net/`
4. Monitoring időköz: 5 perc
5. Ez 5 percenként "pingelni" fogja az alkalmazást, így ébren marad

### 3. **Azure Logic App** (Díj ellenében)
- Cron job-ként hívja meg az alkalmazást
- ~1-2 USD/hó

### 4. **Fizetős App Service Plan**
- Always On funkció elérhető
- ~10-50 USD/hó

## Manuális teszt futtatás
Admin felhasználóként az **Email beállítások > SMTP szerver** fülön található az **"Azonnali futtatás (teszt)"** gomb.