#!/bin/bash

# exit if error
set -o errexit

if [ "$TRAVIS_OS_NAME" = "osx" ]
then
  dotnet_resources_path_in_app=$TRAVIS_BUILD_DIR/breeze_out/breeze-ui-$os_platform-$arch/breeze-ui.app/contents/resources/app/assets/daemon
else
  dotnet_resources_path_in_app=$TRAVIS_BUILD_DIR/breeze_out/breeze-ui-$os_platform-$arch/resources/app/assets/daemon
fi

# define a few variables
app_output_name="breeze-$os_identifier-$arch-$configuration"
api_output_name="api-$os_identifier-$arch-$configuration"

echo "current environment variables:"
echo "OS name:" $TRAVIS_OS_NAME
echo "OS identifier:" $os_identifier
echo "Platform:" $os_platform
echo "Build directory:" $TRAVIS_BUILD_DIR
echo "Node version:" $TRAVIS_NODE_VERSION
echo "Architecture:" $arch
echo "Configuration:" $configuration
echo "App output name:" $app_output_name
echo "Api output name:" $api_output_name
echo "dotnet resources path in app:" $dotnet_resources_path_in_app
echo "Branch:" $TRAVIS_BRANCH
echo "Tag:" $TRAVIS_TAG
echo "Commit:" $TRAVIS_COMMIT
echo "Commit message:" $TRAVIS_COMMIT_MESSAGE


dotnet --info

# Initialize dependencies
echo $log_prefix STARTED restoring dotnet and npm packages
cd $TRAVIS_BUILD_DIR/Breeze
git submodule update --init --recursive

cd $TRAVIS_BUILD_DIR/Breeze.UI

npm install
echo $log_prefix FINISHED restoring dotnet and npm packages

# dotnet build
echo $log_prefix running 'dotnet build'
cd $TRAVIS_BUILD_DIR/StratisBitcoinFullNode/Stratis.BreezeD
dotnet restore -v m
dotnet build -c $configuration -r $os_identifier-$arch -v m 

echo $log_prefix running 'dotnet publish'
dotnet publish -c $configuration -r $os_identifier-$arch -v m -o $TRAVIS_BUILD_DIR/dotnet_out/$TRAVIS_OS_NAME

echo $log_prefix chmoding the Stratis.BreezeD file
chmod +x $TRAVIS_BUILD_DIR/dotnet_out/$TRAVIS_OS_NAME/Stratis.BreezeD

# node Build
cd $TRAVIS_BUILD_DIR/Breeze.UI
echo $log_prefix running 'npm run'
npm run build:prod

# node packaging
echo $log_prefix packaging breeze 
node package.js --platform=$os_platform --arch=$arch --path=$TRAVIS_BUILD_DIR/breeze_out

# copy api libs into app
echo $log_prefix copying the Breeze api into the app
mkdir -p $dotnet_resources_path_in_app
cp -r $TRAVIS_BUILD_DIR/dotnet_out/$TRAVIS_OS_NAME/* $dotnet_resources_path_in_app

# zip result
echo $log_prefix zipping the app into $TRAVIS_BUILD_DIR/breeze_out/$app_output_name.zip
mkdir -p $TRAVIS_BUILD_DIR/deploy/
cd $TRAVIS_BUILD_DIR/breeze_out
zip -r $TRAVIS_BUILD_DIR/deploy/$app_output_name.zip breeze-ui-$os_platform-$arch/*

#tests
echo $log_prefix no tests to run

echo $log_prefix FINISHED build

