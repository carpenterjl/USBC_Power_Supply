import json
import sys
import debugpy

def wait_for_debugger():
    debugpy.listen(("localhost", 5678))
    print("Waiting for debugger", flush=True)
    debugpy.wait_for_client()

def log_to_console(message):
    sys.stdout.write(f"{len(message)}\n{message}")
    sys.stdout.flush()

def send(msg):
    data = json.dumps(msg)
    sys.stdout.write(f"{len(data)}\n{data}")
    sys.stdout.flush()
    response_line = sys.stdin.readline()
    if not response_line:
        raise RuntimeError("Connection to C# application lost")
    return json.loads(response_line)

def set_voltage(voltage, supply):
    response = send({
        "command":"set_voltage",
        "supply":supply,
        "value":voltage
    })
    return response["status"] == "ok"

def read_voltage(supply):
    response = send({
        "command": "read_voltage",
        "supply":supply
    })
    return response["voltage"]

def read_current(supply):
    response = send({
        "command": "read_current",
        "supply":supply
    })
    return response["current"]