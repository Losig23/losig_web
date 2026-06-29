# Portfolio Apartment MVP

The apartment layout generator is installed at:

`Assets/Editor/PortfolioApartmentSceneBuilder.cs`

To generate the scene in Unity:

1. Open this project in Unity.
2. Wait for scripts to finish compiling.
3. Use the menu item `Tools > Portfolio > Build Apartment MVP Scene`.
4. Open `Assets/Scenes/PortfolioApartment.unity`.
5. Press Play.

To refresh only the website link plaques in the saved scene, use `Tools > Portfolio > Add Website Portfolio Placeholders`.

The generated scene includes:

- A photo-inspired apartment layout with open living/office space on the left, the kitchen on the right when entering, the balcony opposite the entry, and the hallway in the gap between the kitchen and futon leading to the bedroom/bathroom behind the kitchen.
- Placeholder geometry for furniture and props.
- Existing project assets where available, including the laptop and free furniture prefabs.
- A first-person Starter Assets player.
- A reusable raycast interaction system.
- A computer monitor that shows `Press E to view resume` and opens the resume PDF.
- Wall plaques for Projects, GitHub, LinkedIn, and Contact that use `PortfolioLinkInteractable`.

Place the resume PDF at:

`Assets/StreamingAssets/Resume.pdf`

If the PDF is missing, pressing `E` logs the expected file path in the Unity Console.
