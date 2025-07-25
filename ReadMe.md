# PlayboxSDK Installation

## Step 1. Add the package to your project

Using Unity Package Manager, add the repository:

```
https://github.com/playbox-technologies/playbox-installer.git
```

## Step 2. Complete installation via PlayboxInstaller

1. In Unity, open the **PlayboxInstaller context menu**.  
2. Go through all stages (Stage 1 → Stage 2).  
3. After completing **Stage 2**, a `DownloadFiles` folder will appear in the root of the project.  
4. Extract the required SDKs from the `DownloadFiles` folder:

   - `Firebase.Analytics`
   - `Firebase.Crashlytics`
   - `FacebookSdk`

## Step 3. Fixing Facebook SDK errors

If errors appear after importing `FacebookSdk`:

1. Delete the `Examples` folder from the Facebook package.
2. In Unity, click: **PlayboxInstaller → Fix Facebook Error**.

## Step 4. Finalize PlayboxSDK installation

As the last step, click:  
**PlayboxInstaller → Install PlayboxSDK**.

After this, PlayboxSDK will be fully installed and ready for use.
