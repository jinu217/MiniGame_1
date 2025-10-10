[터미널에서 깃허브 사용법]

  ## 맨 처음 세팅 (한번만)
cd ..
git clone https://github.com/jinu217/MiniGame_1.git
cd MiniGame_1

  ## develop 다운로드 (PR된 develop 다운시 매번 사용)
git fetch origin
git switch develop
git pull origin develop

  ## 작업완료 후 브랜치 생성 및 푸시
git switch develop
git switch -c (생성할 브랜치명)    # 브랜치명 feature/(작업한 기능)로 통일해서 생성 ex) feature/boss, feature/bug
git add .
git commit -m "feat : (작업한 기능 설명)"    # ex) feat : boss 1 추가, fix : player 버그 수정
git push -u origin (생성할 브랜치명)
