# Setting up development environment

## The development project

As this source code is a standalone package, there must a Unity (2022) Project setup so stuff can be tested and visualized. For this to
work, the following must be accomplished:

- The development project must be at same folder structure level as this source code root foder.
- The development project must have this package intalled through package manager using the "Install from disk" option.
- The development project **MUST** have its root folder called `sprite-animations-development`. This is due to [docfix](https://dotnet.github.io/docfx)
  needing a .csproj to recognize the code API and generate automated documentation.
- Again, because of the [docfix](https://dotnet.github.io/docfx) dependency, Unity must be configured to generate packages csproj files.

## Release it

As we use [release-it](https://github.com/release-it/release-it) to automate releases, Node must be installed so we can run `npm install` in
order to install node dependcies for the [release-it](https://github.com/release-it/release-it) package.

## Releasing

Once everything is set, just run `npm run release` and follow the steps.
