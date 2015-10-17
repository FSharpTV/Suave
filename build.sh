#!/usr/bin/env bash
set -e

echo 'Restoring nugets'
[[ -f tools/paket.exe ]] || mono tools/paket.bootstrapper.exe
mono tools/paket.exe install

echo 'Building'
xbuild src/Web.sln
