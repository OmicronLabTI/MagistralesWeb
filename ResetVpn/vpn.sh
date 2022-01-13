#!/bin/sh

if ping -c1 172.31.55.11 &> /dev/null
then
        echo "Host Found - `date`";
        echo "Done";
else
        echo "Not Host Found - `date`";
        az login --service-principal -u c9c3614b-f51a-469b-906c-527ecce5ce5a -p P7l7Q~TNPU0we1Dlf5kRooMko_g5Ae0o6x7hE --tenant 86df17a5-d3d5-4bd2-8553-7b0d57a3b73c;
        az network vnet-gateway reset -g OmicronUAT -n omicron-uat-gateway;
        echo "Done";
fi
