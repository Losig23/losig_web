# Portfolio Website Next Steps

## 1. Editor Verification

Before publishing, verify the saved Unity scene:

1. Open `Assets/Scenes/PortfolioApartment.unity`.
2. Press Play.
3. Confirm the player starts near the main entrance and stays on the floor.
4. Walk to the desk monitor.
5. Look at the monitor and confirm the prompt appears.
6. Press `E` and confirm the resume PDF opens.
7. Look at the Projects, GitHub, LinkedIn, and Contact wall plaques and confirm prompts appear.

The scene should be first and enabled in `File > Build Profiles`.

## 2. Replace Placeholder Links

Before a public build, select each plaque and update its `PortfolioLinkInteractable` URL:

- Projects: your projects page or hosted project index.
- GitHub: your GitHub profile.
- LinkedIn: your LinkedIn profile.
- Contact: `mailto:your-email@example.com` or a contact page.

For the resume computer, `ResumeComputerInteractable` opens `Assets/StreamingAssets/Resume.pdf` on desktop/editor. In WebGL, StreamingAssets are served as web files, not normal local files, so the code opens a URL instead of checking `File.Exists`.

Use `Web Fallback Url` when you want the WebGL build to open a hosted PDF such as:

```text
https://your-domain.example/resume.pdf
```

Leaving `Web Fallback Url` blank makes WebGL try the build's `StreamingAssets/Resume.pdf` URL.

## 3. Prepare The WebGL Build

In Unity:

1. Open `File > Build Profiles`.
2. Select `Web`.
3. Click `Switch Platform`.
4. Confirm `Assets/Scenes/PortfolioApartment.unity` is first in the scene list.
5. Open Player Settings and set the product/company fields.
6. For GitHub Pages, use `Compression Format: Disabled` for the least server friction, or enable decompression fallback if using Gzip/Brotli.
7. Build into a root-level `docs` folder.

Do not test by double-clicking `docs/index.html`; WebGL and StreamingAssets need a web server.

```powershell
python -m http.server 8000 --directory docs
```

Then open `http://localhost:8000`.

## 4. Recruiter-Friendly Fallback

Add a small static page after the WebGL build exists:

```text
docs/fallback.html
```

Include:

- Name and role.
- Resume PDF link.
- GitHub link.
- LinkedIn link.
- Email/contact link.
- A short note for visitors whose browser blocks WebGL.

This matters because some recruiters use managed machines where WebGL, popups, or built-in PDF viewing may be restricted.

## 5. Hosting Options

GitHub Pages:

1. Build WebGL to `docs`.
2. Commit `docs` only when ready to publish.
3. Push to GitHub.
4. Go to `Settings > Pages`.
5. Select `Deploy from a branch`.
6. Choose `main` and `/docs`.

itch.io:

1. Build WebGL normally.
2. Zip the build output.
3. Create an itch.io project.
4. Set kind to HTML.
5. Upload the zip and mark it playable in browser.

Netlify:

1. Build WebGL to `docs` or another publish folder.
2. Drag the folder into Netlify Drop, or connect the GitHub repo.
3. Set the publish directory to `docs`.
4. Add a custom domain when ready.

## 6. Ongoing Workflow

Use this loop:

1. Work in Unity.
2. Test in the Editor.
3. Build WebGL to `docs`.
4. Test locally with a web server.
5. Commit source changes.
6. Commit `docs` only for a publish update.
7. Push and check the live URL.

## 7. Later Improvements

- Replace placeholder plaques with richer project objects.
- Add in-game project panels for screenshots and descriptions.
- Add a simple loading screen and mobile warning.
- Build a smaller non-Unity fallback homepage.
- Buy a custom domain and point it at GitHub Pages or Netlify.
