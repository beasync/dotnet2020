## Run steps:

1. Run Azure IoT Edge on Ubuntu Virtual Machines:

   https://docs.microsoft.com/es-es/azure/iot-edge/how-to-install-iot-edge-ubuntuvm

2. Create deployment:

   Use Azure Portal or az cli to deploy the solution using the pre-built deployment template, e.g.:

```
az iot edge deployment create -d <DEPLOYMENT NAME> -n <IOT HUB NAME> --content config/deployment.amd64.json --target-condition "deviceId='<YOUR DEVICE ID>'" --priority 10
```

## Acknowledgements

This repository is based on [this sample](https://github.com/vslepakov/iot-edge/tree/master/DaprSample/EdgeActors).