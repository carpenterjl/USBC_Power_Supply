from pythonTranslate import *
import pandas as pd
import matplotlib.pyplot as plt
from openpyxl import load_workbook
from openpyxl.drawing.image import Image
import openpyxl.styles
from openpyxl.worksheet.table import Table
from openpyxl.worksheet.table import TableStyleInfo
import numpy as np
import time

log_to_console("""
========================================
 PYTHON DEBUG MODE (PSU SCRIPT)
========================================

This script is waiting for VS Code to attach.

To debug:

1. Open VS Code
2. Open the folder containing this script
3. Go to Run and Debug (Ctrl + Shift + D)
4. Select: "Attach to PSU Python"
5. Press F5

Once attached:
- Execution will continue automatically
- You can set breakpoints in VS Code
- You can inspect variables and step through code

Waiting for debugger connection...
========================================
""")

wait_for_debugger()
log_to_console("Debugger Connected!")

log_to_console("Starting Test.")

new_rows_list = []

# Number of test samples, taken with a delay of SampleDelay (seconds)
n = 300
Step = 0.05
StartVoltage = 2.5
SampleDelay = 0.1
voltage = read_voltage("N")
start_time = time.perf_counter()

for i in range(n):
    set_voltage((-1)*(StartVoltage + Step*i), "N")
    calculated_voltage = read_voltage("N")
    calculated_current = read_current("N")
    time_snapshot = time.perf_counter()
    time_now = time_snapshot - start_time
    new_rows_list.append({
        "Voltage (V)": calculated_voltage,
        "Target (V)": (-1)*(StartVoltage + Step*i),
        "Current (A)": calculated_current,
        "Sample": i
    })
    time.sleep(SampleDelay)

df = pd.DataFrame(new_rows_list)

# Save spreadsheet
excel_file = "pyReport.xlsx"
df.to_excel(excel_file, index=False, sheet_name="Measurements")

# Generate plot
plt.figure(figsize=(6,4))
plt.plot(df["Sample"], df["Voltage (V)"], marker='o', label="Voltage (V)")
plt.plot(df["Sample"], df["Target (V)"], marker='o', label="Target (V)")
plt.xlabel("Sample")
plt.ylabel("Volts")
plt.title("(Python Generated Graph)" + '\n' + "Voltage")
plt.grid(True)
plt.legend()
plot_fileV = "pyChartVoltage.png"
plt.savefig(plot_fileV)
plt.close()

plt.figure(figsize=(6,4))
plt.plot(df["Sample"], df["Current (A)"], marker='s', label="Current (A)")
plt.xlabel("Sample")
plt.ylabel("Amps")
plt.title("(Python Generated Graph)" + '\n' + "Current")
plt.grid(True)
plt.legend()
plot_fileI = "pyChartCurrent.png"
plt.savefig(plot_fileI)
plt.close()

# Insert plot into Excel
wb = load_workbook(excel_file)
ws = wb.active
img = Image(plot_fileV)
ws.add_image(img, "E2")
img = Image(plot_fileI)
ws.add_image(img, "E22")
wb.save(excel_file)
log_to_console("Report generated")

