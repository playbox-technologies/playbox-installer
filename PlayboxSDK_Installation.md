# Установка PlayboxSDK

## Шаг 1. Добавить пакет в проект

Через Unity Package Manager подключите репозиторий:

```
https://github.com/playbox-technologies/playbox-installer.git
```

## Шаг 2. Выполнить установку через PlayboxInstaller

1. В Unity, откройте **контекстное меню PlayboxInstaller**.  
2. Пройдите все этапы (Stage 1 → Stage 2).  
3. После завершения **Stage 2** в корне проекта появится папка `DownloadFiles`.  
4. Из папки `DownloadFiles` извлеките необходимые SDK:

   - `Firebase.Analytics`
   - `Firebase.Crashlytics`
   - `FacebookSdk`

## Шаг 3. Исправление ошибок Facebook SDK

Если после импорта `FacebookSdk` появляются ошибки:

1. Удалите папку `Examples` из пакета Facebook.
2. В Unity нажмите: **PlayboxInstaller → Fix Facebook Error**.

## Шаг 4. Завершение установки PlayboxSDK

На последнем шаге нажмите:  
**PlayboxInstaller → Install PlayboxSDK**.

После этого PlayboxSDK будет полностью установлен и готов к использованию.
