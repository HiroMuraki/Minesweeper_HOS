MSBuild "/p:Configuration=Release;Optimize=true" "/t:rebuild"
copy "GridGameHOS\bin\Release\GridGameHOS.exe" "$env:userprofile\Desktop\GridGameHOS.exe"
ren "$env:userprofile\Desktop\GridGameHOS.exe" "É¨À×HOS.exe"