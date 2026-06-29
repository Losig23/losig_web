# Apartment Portfolio Game

A Unity 6 URP personal portfolio MVP built as a first-person apartment walkthrough. Visitors can move through the apartment, look at portfolio objects, and press `E` to open resume or profile links.

## Current MVP

- Apartment-inspired first-person scene in `Assets/Scenes/PortfolioApartment.unity`.
- Unity Starter Assets first-person movement.
- Reusable `IInteractable` raycast interaction system.
- Desk computer that opens `Assets/StreamingAssets/Resume.pdf`.
- Website placeholder plaques for Projects, GitHub, LinkedIn, and Contact.
- URP materials only; no HDRP or Built-in-only rendering features.

## Controls

- `WASD`: move
- `Mouse`: look
- `Shift`: sprint
- `Space`: jump
- `E`: interact

## Open And Run

1. Open this folder in Unity `6000.3.11f1` or a compatible Unity 6 version.
2. Open `Assets/Scenes/PortfolioApartment.unity`.
3. Press Play.
4. Confirm the player starts on the entry carpet and does not fall through the floor.
5. Walk to the desk monitor, look at it, and confirm the prompt appears.
6. Press `E` and confirm the resume PDF opens.
7. Look at the wall plaques near the desk to test future website links.

`ProjectSettings/EditorBuildSettings.asset` should keep `PortfolioApartment.unity` first and enabled.

## Important Files

- `Assets/Editor/PortfolioApartmentSceneBuilder.cs` - scene generator and placeholder updater.
- `Assets/Scenes/PortfolioApartment.unity` - saved MVP scene.
- `Assets/Scripts/Interaction/IInteractable.cs` - reusable interaction interface.
- `Assets/Scripts/Interaction/PlayerInteractor.cs` - camera raycast interaction logic.
- `Assets/Scripts/Interaction/InteractionPromptUI.cs` - on-screen prompt UI.
- `Assets/Scripts/Portfolio/ResumeComputerInteractable.cs` - resume computer behavior.
- `Assets/Scripts/Portfolio/PortfolioLinkInteractable.cs` - reusable URL/link interactable.
- `Assets/Scripts/Portfolio/PortfolioUrlOpener.cs` - desktop/WebGL-safe URL helper.
- `Assets/StreamingAssets/Resume.pdf` - resume file opened by the computer.
- `WEBSITE_NEXT_STEPS.md` - hosting and WebGL publishing plan.

## Portfolio Links

The Projects, GitHub, LinkedIn, and Contact plaques use `PortfolioLinkInteractable`. Replace the placeholder URLs in the Inspector before publishing:

- `https://your-domain.example/projects`
- `https://github.com/your-username`
- `https://www.linkedin.com/in/your-profile`
- `mailto:you@example.com`

For the resume computer, keep `Resume File Name` as `Resume.pdf` unless you rename the file. Set `Web Fallback Url` to a hosted PDF URL if you want WebGL builds to open a public resume link instead of the StreamingAssets copy.

## Scene Tools

In Unity:

- `Tools > Portfolio > Build Apartment MVP Scene` regenerates the full scene.
- `Tools > Portfolio > Add Website Portfolio Placeholders` updates only the website plaques in the saved scene.

The second option is safer when you like the current apartment layout and only want to refresh link placeholders.

## WebGL Build

1. Open `File > Build Profiles`.
2. Select `Web`.
3. Click `Switch Platform`.
4. Confirm `Assets/Scenes/PortfolioApartment.unity` is first in the scene list.
5. For GitHub Pages, either disable compression or enable decompression fallback.
6. Build to a root-level `docs` folder.
7. Test with a local server:

```powershell
python -m http.server 8000 --directory docs
```

Then open `http://localhost:8000`.

## Git Notes

Commit source project files:

- `Assets/`
- `Packages/`
- `ProjectSettings/`
- `.gitignore`
- `README.md`
- `WEBSITE_NEXT_STEPS.md`

Do not commit generated local folders:

- `Library/`
- `Temp/`
- `Logs/`
- `UserSettings/`
- `Build/`
- `Builds/`
- `.vs/`
- generated `.csproj` / `.sln` files

Commit `docs/` only when you intentionally want to publish a WebGL build through GitHub Pages.
