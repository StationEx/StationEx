# StationEx

## Overview

StationEx is a feature composition framework for Stationeers. StationEx enables developers to create small, discrete features with exposed configuration. Package authors combine these features and tune their settings to create unique gameplay experiences. When the game updates, the framework handles the bindings — your code keeps working.

## Status

Initial development and design.

## Core Concepts

### Features

A feature is a small, configurable piece of functionality. One assembly can contain one feature. Each feature includes a manifest and configuration template.

### Packages

A package is a curated collection of features with specific configuration values. Package authors select features and provide configuration overrides to create a cohesive gameplay experience.

## License

All components of this project are licensed under [The MIT License](https://opensource.org/license/mit) and are attributed to all of the authors.