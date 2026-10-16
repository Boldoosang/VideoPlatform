# VideoPlatform

VideoPlatform is a web application for turning Microsoft Teams recordings and uploaded videos into publicly viewable episodes. It brings video libraries, browser-based editing, rendering, and episode publishing into one workflow, with Microsoft Entra ID authentication for administrators.

The project began on **3 April 2025** and is described in the application's documentation as **ongoing development**.

## Why VideoPlatform?

Preparing recordings for an audience often means downloading a meeting, editing it in a separate tool, uploading the result, and maintaining a separate publishing page. VideoPlatform is designed to reduce that manual work: administrators prepare content in one application, organize it into seasons or standalone episodes, and publish it for viewers without requiring them to sign in.

## Features

- **Administrator access:** Microsoft Entra ID (Azure AD) sign-in, with management and editing actions restricted to the `Admin` role.
- **Video libraries:** Upload source videos and manage separate original and edited video libraries backed by Azure Blob Storage.
- **Browser-based editing:** Trim and combine clips, add text overlays, and preview edits using an integrated React video editor.
- **Video rendering:** Export edited compositions through a separate Node.js and Remotion service, with progress updates, MP4 output, and generated thumbnails.
- **Episode management:** Create, edit, and delete episodes; maintain titles, descriptions, publish dates, and publication status.
- **Season management:** Organize episodes into seasons or publish them as standalone entries.
- **Public playback:** Browse published content and watch episodes without an account. Administrators can preview unpublished episodes.
- **Administrative dashboard:** Access library, episode, and season management from a central interface.

Microsoft Teams ingestion relies on an external workflow, such as Power Automate, to copy authorized recordings into Azure Blob Storage. The repository does not provide a built-in Teams recording synchronization service.

## Publishing workflow

1. Sign in with an account assigned the `Admin` role.
2. Upload a recording to the Video Library, or make it available through the external Teams-to-storage workflow.
3. Open the recording in the integrated editor, make changes, and give the video a title.
4. Select **Export**. The rendering service creates the video and thumbnail and uploads them to the edited-video container.
5. Create an episode using the edited video, add its metadata, and optionally assign it to a season.
6. Publish the episode and, when applicable, its season so viewers can discover it on the public homepage.

Publication is controlled by the current publication status. Automatic scheduled releases are listed as a future enhancement.

## Architecture and technology

| Component | Responsibility | Main technologies |
| --- | --- | --- |
| Web application | Public pages, administrator dashboard, authentication, and MVC controllers | ASP.NET Core MVC, .NET 8, Razor, Microsoft Identity Web |
| Domain layer | Models, data transfer objects, and repository interfaces | C# |
| Infrastructure layer | Database context, migrations, and repository implementations | Entity Framework Core, SQL Server / Azure SQL |
| Video editor | Interactive editing and composition preview | React 18, TypeScript, Vite, DesignCombo, Remotion |
| Video renderer | Render jobs, MP4 export, thumbnails, and storage uploads | Node.js, Express, Remotion, Sharp |
| Media storage | Original recordings, edited videos, and thumbnails | Azure Blob Storage |
| Web interface | Layout, tables, and client-side interactions | Bootstrap, jQuery, DataTables |

Azure App Service is the hosting platform described in the application documentation.

The editor submits a JSON video design to the rendering service and polls for progress. The renderer produces the media and uploads it to Blob Storage. The MVC application manages episode and season metadata in SQL Server and presents published content to viewers.

### Repository layout

```text
VideoPlatform.sln
VideoPlatform.Web/             ASP.NET Core MVC application and static assets
VideoPlatform.Domain/          Models, DTOs, and interfaces
VideoPlatform.Infrastructure/  EF Core context, migrations, and repositories
videoplatform.videoeditor/     React and TypeScript video editor
VideoPlatform.VideoRenderer/   Node.js and Remotion rendering service
```

The application documentation also includes [architecture](https://github.com/Boldoosang/VideoPlatform/blob/main/VideoPlatform.Web/wwwroot/images/arch.png), [database design](https://github.com/Boldoosang/VideoPlatform/blob/main/VideoPlatform.Web/wwwroot/images/cd.png), [video editing flow](https://github.com/Boldoosang/VideoPlatform/blob/main/VideoPlatform.Web/wwwroot/images/sf-ve.png), [use cases](https://github.com/Boldoosang/VideoPlatform/blob/main/VideoPlatform.Web/wwwroot/images/uc.png), and [interface wireframes](https://github.com/Boldoosang/VideoPlatform/blob/main/VideoPlatform.Web/wwwroot/images/ui.png).

## Development setup

### Prerequisites

- .NET 8 SDK.
- Node.js and npm for the editor and renderer. The renderer Dockerfile uses Node.js 20.
- A SQL Server or Azure SQL database.
- An Azure Storage account with `videos` and `editedvideos` Blob containers.
- A Microsoft Entra ID app registration and an administrator account assigned the `Admin` app role.
- A Chromium environment suitable for the renderer, or Docker to use the included renderer image.

### Clone the repository

```bash
git clone https://github.com/Boldoosang/VideoPlatform.git
cd VideoPlatform
```

### Configure the web application

Use .NET user secrets for local credentials or environment variables in the hosting environment. The web project already declares a user-secrets ID.

| Configuration key | Purpose |
| --- | --- |
| `AzureAd:Instance` | Microsoft identity authority; the checked-in value is `https://login.microsoftonline.com/`. |
| `AzureAd:Domain` | Your Entra tenant domain. |
| `AzureAd:TenantId` | Your tenant ID. |
| `AzureAd:ClientId` | Your app registration's client ID. |
| `AzureAd:CallbackPath` | Sign-in callback path; the checked-in value is `/signin-oidc`. |
| `ConnectionStrings:DatabaseConnection` | SQL Server connection string. |
| `ConnectionStrings:AzureBlobStorage` | Azure Storage connection string. |
| `VideoRendererBackendUrl` | Renderer base URL used by the edited-library page. |

For example, set the database connection from the repository root:

```bash
dotnet user-secrets set "ConnectionStrings:DatabaseConnection" "<sql-connection-string>" --project VideoPlatform.Web
```

Configure the remaining values in the same way, including any credentials required by your Entra app registration. For environment variables, replace `:` with `__`, such as `ConnectionStrings__DatabaseConnection`. Keep credentials out of source control.

For the checked-in HTTPS launch profile, register `https://localhost:7050/signin-oidc` as the web redirect URI. Ensure the storage containers exist and that media URLs can be read by the browser and rendering service; the current implementation returns direct Blob URLs.

### Configure and run the renderer

The renderer reads the following environment variables and also loads a local `.env` file through `dotenv`:

| Variable | Purpose |
| --- | --- |
| `AZURE_STORAGE_CONNECTION_STRING` | Connection string for the media storage account. |
| `AZURE_STORAGE_CONTAINER_NAME` | Output container; use `editedvideos` to match the web application's edited library. |
| `CHROMIUM_EXECUTABLE_PATH` | Chromium executable path used by the video-rendering call. |
| `PORT` | HTTP listening port; defaults to `8080`. |

```bash
cd VideoPlatform.VideoRenderer
npm install
npm start
```

Keep this service running in a separate terminal. A Dockerfile is included for a Linux environment with Chromium; its installed Chromium path is `/usr/bin/chromium`. Set `CHROMIUM_EXECUTABLE_PATH` explicitly for that environment.

### Configure the editor and start the web app

Set `VITE_VIDEO_RENDERER_BACKEND` in `videoplatform.videoeditor/.env.local` to the renderer URL reachable by the browser. Set the web application's `VideoRendererBackendUrl` to the same service. Use an HTTPS renderer endpoint when serving the application over HTTPS.

Build the editor from the repository root:

```bash
cd videoplatform.videoeditor
npm install
npm run build
cd ..
dotnet run --project VideoPlatform.Web --launch-profile https
```

Vite writes the editor build to `VideoPlatform.Web/wwwroot`; the MVC editor action serves its `index.html`. The web project also attempts an editor install and build during its own build.

Open `https://localhost:7050`. The application attempts to apply EF Core migrations at startup, so the configured database account needs the appropriate permissions. Review startup logs if migration or connection errors occur.

### Existing integration considerations

- Repository paths have inconsistent casing: the editor directory is tracked as `videoplatform.videoeditor`, while solution/build references use `VideoPlatform.VideoEditor`. Two infrastructure project paths also differ only by casing. These existing inconsistencies need attention when preparing a build on a case-sensitive filesystem; Git may report a checkout collision on Windows.
- The web project's editor build target continues after an editor failure. Check that the frontend build succeeds and produces the expected files before using the editor.
- In-editor media uploads currently use external upload services, including `tmpfiles.org`. Uploads through the MVC video library use Azure Blob Storage.
- Renderer job progress is stored in memory, so restarting the renderer clears its job status history.

## Documentation and testing

The primary project write-up is the application's **Technical Documentation** page at `/Home/Documentation`. Its [source](https://github.com/Boldoosang/VideoPlatform/blob/main/VideoPlatform.Web/Views/Home/Documentation.cshtml) contains the problem statement, requirements, design diagrams, technology choices, and manual acceptance scenarios.

That page records 13 manual test cases covering administrator login, editing and rendering, season and episode management, public playback, draft access, and library deletion safeguards. These are recorded project results, not an automated test suite or a guarantee that a new environment has been validated.

## Planned enhancements

The application documentation identifies these future directions:

- AI-powered content summarization.
- Automated transcription.
- Scheduled episode releases.
- A richer or replaceable video editor.

## Acknowledgments

The integrated editing and rendering work builds on projects credited in the application documentation:

- [DesignCombo React Video Editor](https://github.com/designcombo/react-video-editor).
- [DesignCombo Video Renderer Site](https://github.com/designcombo/video-renderer-site).

The editor includes its own [license file](https://github.com/Boldoosang/VideoPlatform/blob/main/videoplatform.videoeditor/LICENSE). The repository currently has no root-level license file.
