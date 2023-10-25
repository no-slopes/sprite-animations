@echo off

SET root=%~dp0
SET startPath=Samples
SET destPath=_SpriteAnimations\Samples~

echo %root%
echo StartPath is: %startPath%
echo DestPath is: %destPath%

mklink /D %startPath% %destPath%
pause