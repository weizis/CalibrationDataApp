# Space — Solar Sensor Calibration System

WPF application for calibration and visualization of solar sensor data,  
developed as part of the "My Profession — IT" hackathon.

⚠️ This project uses experimental data from the **ObriCraft** solar sensor.  
Real data **is not included** in the repository and is not publicly shared.

---

## 🚀 Features
- Load measurements from solar sensor (Excel and TXT formats)
- Linear calibration of signals (offset and scale)
- Data visualization
- Comparison of raw and calibrated signals

---

## 🧠 Calibration Model

A linear model is used:
X_calibrated = k * X + b
Y_calibrated = k * Y + b


The model compensates for:
- Zero offset  
- Scale error  

> Temperature drift is not considered in this version.

---

## 📊 Data

The application expects input data in **TXT** or **Excel** format, compatible with **ObriCraft** sensor exports.

### Folder Structure
`````
Space/
├─ Data/ ← synthetic sample data
│ └─ calibration.txt
├─ Views/
├─ Model/
├─ Service/
├─ Excel/
├─ Fonts/
└─ Resources/
`````

### TXT Format

| Field  | Description     |
|-------:|----------------|
| Time   | Timestamp       |
| Sun1X  | Signal X-axis   |
| Sun1Y  | Signal Y-axis   |

Example content of `calibration.txt`:

---

## 🖼 Interface
- Calibration page  
- Display of calibration coefficients  
- Graphs of raw and calibrated signals  

---

## 🛠 Technologies
- C#  
- .NET  
- WPF  
- MVVM  
- OxyPlot  
- Excel and TXT data handling  

---

## 📦 Architecture
- **Model** — data models  
- **Service** — business logic and calibration  
- **Views** — user interface  
- **Excel** — data import  

---

## ▶️ Running the Project

1. Open `Space.sln` in Visual Studio  
2. Restore NuGet packages  
3. (Optional) Use the sample data from `/Data/calibration.txt`
4. Run the application  

---

## 👥 Team Work
- Role: **Team Lead / Backend Developer**  
- Organized work using SCRUM  
- Communicated with experts  
- Presented the project to the jury  

---

## 🏆 Achievement
Third place in the regional stage of the "My Profession — IT" hackathon



