@echo off
echo [🧹 Git 정리 스크립트 실행 중...]

REM Git 캐시에서 불필요한 파일 제거
git rm --cached "API 키.txt"
git rm --cached "구글 앱.txt"
git rm --cached "settings.txt"
git rm --cached "chromedriver.exe"

REM 빌드 결과물 및 임시 폴더 제거
git rm -r --cached bin
git rm -r --cached obj
git rm -r --cached .tmp.driveupload

REM 커밋 메시지와 함께 정리 내용 반영
git commit -m "🧹 정리: 불필요한 파일 및 빌드 출력물 제거"

echo [✅ 정리 완료 - Git에 반영되었습니다.]
pause