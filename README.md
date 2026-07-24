# Luminia

순환 왕정이 무너진 대륙을 되찾는 Windows용 도트 전략 오토배틀 RPG의 첫 플레이 가능 프로토타입입니다.

> Windows에서는 프로젝트의 `START-LUMINIA.bat`을 더블클릭하세요. `Luminia executable was not found`와 Unity 메뉴를 직접 열라는 3단계 안내가 나오면 **구버전 프로젝트**입니다. 최신 ZIP을 새 폴더에 전체 압축 해제해야 합니다.

## 현재 들어 있는 내용

- 타이틀 화면과 세계관 프롤로그
- 자이언트·엘프·인간·마족·종합지구로 구성된 대륙 지도
- 제공된 고지도 컨셉 이미지를 사용하는 클릭형 대륙 지도
- 인간령 외곽 첫 전투
- 골드를 사용하는 무작위 영웅 소환
- 이름과 등급이 같은 영웅의 합성
- 런타임 도형이 아닌 별도 PNG 스프라이트 에셋을 사용하는 6종 캐릭터
- 하나로 연결된 석재 전장과 성벽, 명령 패널, 입체 테두리 버튼 HUD
- 영웅과 마족 선봉대가 직접 움직이며 공격하는 시각화된 자동 전투
- 근접 돌진, 원거리 투사체, 피해 숫자, 체력바, 피격·사망 연출
- 3회마다 발동하는 강화 스킬, 화염 광역 공격과 치유사 회복
- 게임 내 버튼으로 열 수 있는 별도 진단 로그 폴더

캐릭터는 런타임 코드로 픽셀을 생성하지 않습니다. 프로젝트에 포함된 독립 PNG 스프라이트 에셋을 사용하며, 수호기사·궁수·치유사·화염술사·마족 보병·마족 사수는 서로 다른 갑옷, 무기, 실루엣을 가집니다. PR 전송을 위해 원본은 `SourceAssets/Units/*.png.b64` 텍스트로 보관하고 Unity가 PNG로 자동 복원합니다.

컨셉 지도는 PR 시스템이 PNG 바이너리를 거부하지 않도록 `SourceAssets/world_map.png.b64` 텍스트로 보관합니다. Unity가 프로젝트를 열면 `WorldMapInstaller`가 이를 `Assets/Resources/Art/world_map.png`로 자동 복원하고 Sprite로 설정합니다. 사용자가 지도를 직접 복사할 필요는 없습니다.

## 필요한 프로그램

1. [Unity Hub](https://unity.com/download)
2. Unity Editor `6000.0.35f1` (Unity 6 LTS 계열)
3. 설치 모듈에서 `Windows Build Support (IL2CPP)` 또는 Windows 빌드 지원 선택

프로젝트는 Unity Personal로 열 수 있습니다. 개인용 로컬 프로토타입을 실행하기 위해 별도의 유료 에셋은 필요하지 않습니다.

## Unity Editor에서 바로 실행하기

1. Unity Hub를 실행합니다.
2. `Add` 또는 `Open`을 눌러 이 저장소 폴더를 선택합니다.
3. Unity가 패키지 가져오기를 마칠 때까지 기다립니다.
   - 이때 Console에 `Restored generated world map`이 한 번 표시될 수 있으며 정상입니다.
4. Project 창에서 `Assets/Scenes/Main.unity`를 엽니다.
5. 에디터 상단의 ▶ Play 버튼을 누릅니다.

첫 실행에서는 Unity가 `Library` 폴더를 생성하므로 몇 분 정도 걸릴 수 있습니다.

> `All compiler errors have to be fixed before you can enter playmode!`가 보이면 정상 게임 화면이 아닙니다. 아래 `컴파일 오류가 표시될 때` 절차를 먼저 따라야 합니다.

## Windows에서 가장 쉽게 실행하기

Unity 화면을 직접 조작하기 어렵다면 다음 방법만 사용하면 됩니다.

1. Unity Hub에서 Unity 6을 한 번만 설치합니다.
2. Unity와 Unity Hub 창을 모두 닫습니다.
3. 프로젝트 폴더의 `START-LUMINIA.bat`을 더블클릭합니다.
4. Windows에서 확인 창이 나타나면 `실행`을 누릅니다.
5. 첫 실행은 자동 빌드 때문에 몇 분 걸릴 수 있습니다.
6. 빌드가 끝나면 `Luminia.exe`가 자동으로 실행됩니다.

두 번째 실행부터는 이미 생성된 게임을 바로 실행하므로 Unity 빌드를 기다리지 않아도 됩니다. 실행 파일은 `Build/Windows/Luminia.exe`에 생성됩니다. `run-luminia.bat`도 동일하게 동작하지만, 구버전 파일과 혼동을 피하기 위해 `START-LUMINIA.bat` 사용을 권장합니다.

다음 문구가 표시되면 자동 실행기가 들어오기 전의 오래된 `run-luminia.bat`을 실행한 것입니다.

```text
Luminia executable was not found.
1. Open this folder in Unity Hub ...
2. In Unity, choose: Luminia > Build Windows
```

이 경우 기존 폴더에 일부 파일만 덮어쓰지 말고 최신 프로젝트 ZIP을 다시 받은 뒤 **새 폴더에 전체 압축 해제**하고, 새 폴더의 `START-LUMINIA.bat`을 실행합니다.

스크립트는 Unity Hub의 기본 설치 폴더에서 가장 최신 Unity Editor를 자동으로 찾습니다. Unity를 찾지 못하면 Unity Hub 다운로드 페이지를 열고, 빌드에 실패하면 `Build/unity-build.log` 경로를 알려 줍니다.

## Windows 실행 파일 만들기

Unity 메뉴에서 다음을 선택합니다.

```text
Luminia > Build Windows
```

완료되면 다음 파일이 생성됩니다.

```text
Build/Windows/Luminia.exe
```

그 뒤 `run-luminia.bat`을 더블클릭하거나 `Luminia.exe`를 직접 실행합니다. `Luminia.exe`만 따로 옮기지 말고 `Build/Windows` 폴더 전체를 함께 보관해야 합니다.

명령줄 빌드가 필요하면 Unity 설치 경로를 환경에 맞게 바꿔 다음 명령을 사용할 수 있습니다.

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.35f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath "$PWD" `
  -executeMethod Luminia.Editor.BuildWindows.BuildFromCommandLine `
  -logFile "Build\unity-build.log"
```

## 정상적으로 보이는 화면

### 1. 타이틀

- 어두운 청색 배경 중앙에 `LUMINIA` 제목이 표시됩니다.
- 부제는 `무너진 순환 왕정`입니다.
- `프롤로그 시작`, `오류 로그 폴더`, `종료` 버튼이 보여야 합니다.

### 2. 프롤로그

- 인간 왕 암살과 마족의 종합지구 점령을 설명하는 문장이 표시됩니다.
- 아래의 `대륙 지도` 버튼으로 다음 화면에 들어갈 수 있어야 합니다.

### 3. 대륙 지도

- 북서 자이언트, 북동 엘프, 남동 인간, 남서 마족 지역이 서로 다른 색으로 표시됩니다.
- 중앙에는 잠긴 최종 목표 `종합지구`가 있습니다.
- 현재 플레이 가능한 `인간령`을 누르면 전투 화면으로 이동합니다.

### 4. 전투

- 위쪽 붉은 영역에는 `마족 보병`과 `마족 사수`가 표시됩니다.
- 아래쪽 푸른 영역에는 기본 영웅 `수호기사`와 `숲의 궁수`가 표시됩니다.
- `영웅 소환 5G`를 누르면 골드가 줄고 영웅이 추가됩니다.
- 같은 영웅이 둘 있으면 `같은 영웅 합성`으로 등급과 능력치가 올라갑니다.
- `전투 시작`을 누르면 근접 영웅이 적에게 돌진하고, 궁수·마법사는 투사체를 발사합니다.
- 공격 시 피해 숫자와 피격 흔들림이 표시되고 체력바가 실제로 감소합니다.
- 세 번째 공격마다 강화 스킬이 발동하며, 화염술사는 광역 피해를 주고 치유사는 부상당한 아군을 회복합니다.
- 체력이 0이 된 유닛은 어두워지고 투명해지며, 모든 유닛이 쓰러졌을 때 승패가 결정됩니다.

## 오류를 전달하는 방법

게임은 실행할 때마다 별도 로그 파일을 만듭니다.

```text
%USERPROFILE%\AppData\LocalLow\SoloDeveloper\Luminia\Logs
```

다음 방법 중 하나로 폴더를 열 수 있습니다.

- 게임 화면 왼쪽 아래 `오류 로그 폴더` 또는 전투 화면의 `오류 로그` 버튼 클릭
- 프로젝트 루트의 `open-error-logs.bat` 더블클릭
- Windows 탐색기 주소창에 위 경로 붙여넣기

문제가 생기면 다음 세 가지를 함께 전달하면 됩니다.

1. 가장 최근의 `Luminia_날짜_시간.log`
2. 오류 화면 스크린샷
3. 오류 직전에 누른 버튼과 재현 순서

Unity Editor에서만 발생한 오류라면 Console 창의 빨간 오류를 선택한 뒤 `Ctrl+C`로 복사해 함께 전달합니다. Windows 빌드 과정에서 실패했다면 `Build/unity-build.log`도 함께 전달합니다.

### 컴파일 오류가 표시될 때

1. Unity 아래쪽의 `Console` 탭을 클릭합니다.
2. 빨간색 오류 중 가장 위의 항목을 클릭합니다.
3. `Ctrl+C`로 복사해 전달합니다.
4. Unity가 한 차례 컴파일을 완료했다면 프로젝트의 `DiagnosticLogs/UnityCompilerErrors.txt`도 함께 전달합니다.
5. 로그 파일은 Unity 메뉴의 `Luminia > Open Compiler Error Log`로 열 수 있습니다.

Console의 빨간 오류가 보이도록 캡처한 스크린샷도 원인 확인에 충분히 도움이 됩니다. 가능하면 오류 목록의 첫 번째 빨간 항목과 아래쪽 상세 메시지가 함께 보이도록 캡처합니다.

코드를 수정한 새 버전을 받은 뒤에도 이전 오류가 남으면 Unity를 종료하고 프로젝트의 `Library` 폴더를 삭제한 다음 다시 열어 패키지와 스크립트를 재임포트합니다. `Assets`, `Packages`, `ProjectSettings` 폴더는 삭제하면 안 됩니다.

지도가 보이지 않을 때에는 Unity 메뉴의 `Luminia > Restore World Map`을 한 번 누릅니다. 원본 텍스트가 정상이라면 지도 PNG가 자동으로 다시 생성됩니다.

로그에는 게임 버전, Unity 버전, 운영체제, CPU, GPU, 해상도와 Unity의 오류 스택 추적이 기록됩니다. 계정 비밀번호나 개인 문서는 수집하지 않습니다.

## 조작

- 마우스 왼쪽 버튼: 메뉴 및 전투 버튼 선택
- `Alt+F4`: 게임 종료

이 버전은 시스템 구조를 확인하는 첫 프로토타입입니다. 영웅 드래그 배치, 저장, 설정, 사운드, 실제 도트 애니메이션과 나머지 지역 캠페인은 다음 단계에서 확장할 항목입니다.
