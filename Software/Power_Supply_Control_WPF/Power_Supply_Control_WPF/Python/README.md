# Python Scripting Environment Setup

The power supply software supports Python-based automation, measurement collection, data analysis, and report generation.

Python scripts run inside an isolated virtual environment located within the application folder. This allows users to install additional packages without affecting their system-wide Python installation.

Note: The virtual environment (Python/Environment) is intentionally excluded from source control and must be created locally on each machine.

---

## Folder Structure

```text
Python/
│
├── Environment/
│   ├── Scripts/
│   ├── Lib/
│   └── ...
│
├── Scripts/
│   ├── pythonTranslate.py
│   ├── requirements.txt
│   ├── user_script.py
│   └── ...
│
└── Reports/
```

### Folder Descriptions

| Folder        | Description                          |
| ------------- | ------------------------------------ |
| `Environment` | Python virtual environment           |
| `Scripts`     | User scripts and helper modules      |
| `Reports`     | Generated reports and exported files |

---

# Prerequisites

Install Python 3.10 or newer.

During installation, ensure **"Add Python to PATH"** is enabled.

Verify installation:

```bash
python --version
```

Example:

```text
Python 3.12.4
```

---

# Creating the Virtual Environment

Open a Command Prompt and navigate to the application's Python folder:

```bash
cd C:\PowerSupply\Python
```

Create the virtual environment:

```bash
python -m venv Environment
```

This creates:

```text
Python/
└── Environment/
    ├── Scripts/
    ├── Lib/
    └── ...
```

---

# Activating the Virtual Environment

Activate the environment:

```bash
Environment\Scripts\activate
```

The command prompt should now display:

```text
(Environment) C:\PowerSupply\Python>
```

---

# Installing Required Packages

The `Scripts` folder contains a `requirements.txt` file listing all required Python packages.

Example:

```text
numpy
pandas
matplotlib
openpyxl
debugpy
```

Install all dependencies:

```bash
pip install -r Scripts\requirements.txt
```

Verify installation:

```bash
pip list
```

---

# Adding User Scripts

Place all Python automation scripts inside:

```text
Python/Scripts/
```

Example:

```text
Python/
└── Scripts/
    ├── pythonTranslate.py
    ├── requirements.txt
    ├── voltage_sweep.py
    ├── efficiency_test.py
    └── load_regulation_test.py
```

---

# Using the Power Supply API

The `pythonTranslate.py` helper module provides access to power supply functions.

Example:

```python
from pythonTranslate import *

set_voltage(12, "P")

voltage = read_voltage("P")
current = read_current("P")

print(f"Voltage: {voltage} V")
print(f"Current: {current} A")
```

The helper functions automatically communicate with the main application using the internal JSON messaging protocol.

---

# VS Code Debugging (Optional)

Install the Python extension for Visual Studio Code.

Create a `.vscode/launch.json` file:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Attach to PSU Python",
            "type": "debugpy",
            "request": "attach",
            "connect": {
                "host": "localhost",
                "port": 5678
            }
        }
    ]
}
```

Scripts that call:

```python
wait_for_debugger()
```

will pause execution until VS Code attaches.

After attaching, standard debugging features are available:

* Breakpoints
* Step Into
* Step Over
* Variable Inspection
* Call Stack Navigation

---

# Running Scripts

1. Launch the Power Supply application.
2. Select a script from the `Python/Scripts` folder.
3. Click **Run Script**.

The application automatically:

* Launches the Python interpreter from the virtual environment
* Starts the selected script
* Redirects stdin/stdout communication
* Processes power supply commands
* Returns responses to the script

No manual activation of the virtual environment is required when running scripts through the application.

---

# Updating Installed Packages

Activate the environment:

```bash
Environment\Scripts\activate
```

Install additional packages:

```bash
pip install scipy
```

Update the requirements file:

```bash
pip freeze > Scripts\requirements.txt
```

---

# Example Requirements File

```text
numpy
pandas
matplotlib
openpyxl
debugpy
scipy
```

### Package Usage

| Package    | Purpose                             |
| ---------- | ----------------------------------- |
| NumPy      | Numerical calculations              |
| Pandas     | Data collection and analysis        |
| Matplotlib | Plot generation                     |
| OpenPyXL   | Excel report generation             |
| DebugPy    | VS Code debugging support           |
| SciPy      | Advanced analysis and curve fitting |

---

# Example Workflow

```text
User Script
      │
      ▼
Power Supply API
      │
      ▼
Collect Measurements
      │
      ▼
Pandas DataFrame
      │
      ├── Statistics
      ├── Graphs
      └── Excel Report
```

Typical automated test flow:

1. Set output voltage/current
2. Collect measurements
3. Store results in a DataFrame
4. Generate statistics
5. Create graphs
6. Export a formatted Excel report

```
```
