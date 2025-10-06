# Bezier Spline Project

## Overview

The Bezier Spline Project is a Unity-based tool for creating and manipulating smooth Bezier curves in both 2D and 3D space. Whether you're designing paths for moving objects, generating procedural shapes, or visualizing mathematical curves, this package provides an intuitive and flexible solution.

<img width="1020" height="699" alt="Bezier Spline Example" src="https://github.com/user-attachments/assets/fc7e25ab-d2ae-4bef-b960-699215b808a7" />

---

## Features

- **Bezier Spline Implementation:** Robust C# code for generating and evaluating Bezier curves within Unity.
- **Editor Integration:** Easily adjust curve control points directly in the Unity editor for immediate visual feedback.
- **Loop Support:** Optionally enable looping for closed curves—great for circular paths or repeating animations.
- **Customizable:** Fully extendable for custom behaviors and curve manipulations.

---

## Requirements

- **Unity:** Version 2021 or later
- **Scripting:** C# language

---

## Getting Started

1. **Clone the Repository:**  
   Download or clone this project to your local machine.
2. **Open in Unity:**  
   Launch Unity and open the project folder.
3. **Find the Script:**  
   Go to `Assets/Scripts/BezierSpline.cs` to review or modify the spline logic.
4. **Set Up Your Scene:**  
   - Add a new GameObject to your scene.
   - Attach the `BezierSpline` script component.
   - Adjust control points and the `loop` setting as needed via the inspector.

---

## Usage

- **Curve Points:**  
  Modify the `points` array in the inspector to shape your spline.
- **Looping:**  
  Use the `loop` toggle to create seamless, closed curves.
- **Extending:**  
  Integrate with custom scripts to animate objects along the curve, generate mesh geometry, or drive gameplay logic.

---

## Contributing

This project is actively maintained and open to improvement!  
- **Issues:** Found a bug or have a feature request? Open an issue.
- **PRs:** Pull requests for enhancements and fixes are welcome.

---

## License

This project is licensed under the MIT License.  
See the [LICENSE](LICENSE) file for full details.

---

## Acknowledgments

Thanks to the Unity community for inspiration and resources related to spline implementation.
