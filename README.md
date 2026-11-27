# Portfolio

Unity 6 기반으로 제작한 개인 게임 클라이언트 포트폴리오 프로젝트입니다.

단순한 기능 구현보다 **구조 설계, 의존성 관리, 유지보수성, 확장성**에 중점을 두고 개발하고 있습니다.

실제 게임 프로젝트에서 발생할 수 있는 요구사항을 가정하여 **DI 기반 클라이언트 구조, MVP 패턴, Addressables, ScriptableObject 데이터 설계, 서버-클라이언트 구조(Mock), Object Pooling, UI Framework, 전투 시스템, 가챠/상점/용병 성장 시스템, 로컬라이제이션** 등을 구현했습니다.

---

# Preview

* Unity 6 기반 모바일 게임 클라이언트 (Android Portrait)
* 서바이벌 형태의 전투 (조이스틱 이동 / Dash / 자동 스킬 / 속성 드롭 아이템)
* 용병 획득 · 레벨업 · 등급업(조각) 성장 시스템
* 상점(일일/평생 구매 제한) 및 가챠(가중치 기반 뽑기)
* MockGameServer 기반 서버-클라이언트 구조 (JSON 파일 영속화)
* Addressables 기반 리소스 관리 + Unity Localization (ko / en)
* DI + MVP 기반 UI 구조
* Object Pooling
* ShaderLab / HLSL 기반 커스텀 셰이더

---

# Tech Stack

| Category     | Stack                                          |
| ------------ | ---------------------------------------------- |
| Engine       | Unity 6000.6.3f1                               |
| Language     | C#                                             |
| Rendering    | URP / ShaderLab / HLSL                         |
| Platform     | Android                                        |
| Architecture | DI / MVP / Session & Context                   |
| Data         | ScriptableObject / PVO / Newtonsoft.Json       |
| Asset System | Addressables                                   |
| Async        | UniTask / async-await                          |
| UI           | Unity UGUI / TextMeshPro                       |
| Input        | Unity Input System / On-screen Joystick        |
| Localization | Unity Localization (SmartFormat)               |
| Animation    | DOTween / Animator                             |

---

# Architecture

## Context / Session 기반 클라이언트 구조

Singleton 중심의 접근에서 벗어나, 수명(Lifetime)별로 Context를 분리하여 의존성을 관리합니다.

```text
AppRoot (static)
 └── AppContext                    앱 수명 (GlobalRoot, DontDestroyOnLoad)
      ├── IAssetService
      ├── Storage / SaveService    로컬 설정 저장
      ├── LocaleService
      ├── ISoundService
      └── IDeviceSettings

GameSession                        로그인 ~ 로그아웃 수명
 └── GameContext
      ├── IGameServer (MockGameServer)
      ├── User / ICurrencyService
      ├── MercenaryService / PurchaseService / GachaService / StageService
      ├── MonsterCatalog / ProjectileCatalog / DropItemCatalog
      ├── ISceneService / IPopupService
      └── GameDataLoader

BattleSession                      전투 1회 수명
 ├── BattleState / BattleFlow / StageRunner
 ├── ActorWorld (Spawner 묶음)
 └── BattleContext (IBattleEnvironment)
```

* `AppContext`: 앱 전체 수명 객체를 소유. `AppRoot.EnsureCreated`가 보존/중복 파괴 담당
* `GameSession`: 세션 시작/종료 시 `GameContext`를 생성/해제
* `BattleSession`: 전투 시작 시 생성, 종료 시 `CancellationToken` 취소 및 월드 정리
* 각 Scene / Presenter는 `Deps` 객체로 필요한 서비스만 명시적으로 주입받음
* 인터페이스(`IAssetService`, `ISceneService`, `IPopupService`, `ISoundService`, `IBattleState` 등)를 통한 서비스 추상화

## Boot Flow

`BootLauncher`가 가중치가 있는 단계별 초기화를 수행하고, `BootProgress`가 진행률을 보간하여 UI에 전달합니다.

```text
Addressables Init (25%)
  → Localization Init (10%)
  → Preload Assets (25%, "Preload" 라벨)
  → Load Data (20%, 테이블 병렬 로드 + Storage 로드)
  → Server Init (20%, InitializeAsync → PVO 반영)
```

단계 실패 시 `BootException`으로 실패 지점을 식별할 수 있습니다.

---

# Server / Client

실제 서버가 없는 개발 환경에서도 서버-클라이언트 구조를 유지할 수 있도록 `MockGameServer`를 구현했습니다.

```text
Client (Service)
  │  Request
  ▼
IGameServer
  │
  ▼
MockGameServer
  ├── Handler : 요청 검증 / 결과 코드 / Response 조립
  │     ├── MockUserHandler
  │     ├── MockMercenaryHandler
  │     ├── MockPurchaseHandler
  │     └── MockGachaHandler
  └── Domain  : 데이터 변경 / 저장
        ├── MockUserDomain
        ├── MockMercenaryDomain
        ├── MockPurchaseDomain
        └── MockGachaDomain
              └── JsonFileStorage / JsonListStorage (tmp 쓰기 → Replace/Move 원자적 저장)
```

클라이언트가 로컬 데이터를 직접 수정하지 않고 **Request → 서버 검증 → Response(PVO) → 클라이언트 반영** 흐름을 따르도록 구현하여, 이후 실제 네트워크 서버로 교체할 수 있도록 구성했습니다.

### 주요 기능

* 서버 요청 / 응답 구조 (`Request` / `Response` / `NetworkResult`)
* 클라이언트 사전 검증(`Result` enum) + 서버 검증(`NetworkResult`) 이중 구조
* Response 기반 PVO 갱신
* 서버 결과 코드 → 클라이언트 결과 / 로컬라이즈된 에러 팝업 매핑
* 일일 상품 구매 횟수 UTC 기준 자동 초기화

---

# Player Data

게임 데이터를 런타임 객체와 분리하기 위해 PVO 구조를 사용했습니다.

```text
InitializeResponse
 ├── UserPVO
 ├── MercenaryPVO
 ├── ProductPVO
 └── GachaPVO
```

서버 응답으로 전달받은 데이터를 클라이언트 상태에 반영하는 형태로 구성했습니다. `User`는 PVO 반영 시 Gold/Gem/Exp 변경 이벤트를 발행하고, `CurrencyService`가 이를 구독하여 UI에 재화 변화를 전달합니다.

---

# Mercenary System

용병 데이터를 `ScriptableObject` 기반 Definition과 런타임 PVO로 분리했습니다.

```text
MercenaryDefinition (ScriptableObject)
        │
        ▼
    Mercenary (Role<T>)  ◀── ApplyPVO ── MercenaryPVO
```

### 구현 기능

* 용병 목록 및 보유 여부 관리 / 획득(기본 지급, 골드 구매, 가챠 조각)
* 레벨업 (1레벨 / 길게 눌러 연속 / 최대 레벨까지)
* 레벨 초기화 (사용한 골드 환급)
* 등급업 (조각 누적 기반, 진행도 표시)
* 유저 레벨 / 보유 골드에 따른 최대 레벨 계산
* 성장 스탯 (`value + growth * (level - 1)`) 및 전투력 계산(`CombatPowerCalculator`)
* 스킬 트리 및 스킬 정보 표시, 좌우 스와이프로 용병 전환

### Level / Grade System

레벨/등급 규칙을 `MercenaryLevelSystem`, `MercenaryGradeSystem`으로 분리하여 클라이언트 사전 검증과 서버 검증이 **같은 규칙**을 사용하도록 했습니다.

```text
UserLevel / GameConfig.MercenaryLevelLimit / Level Table
     ▼
AvailableMaxLevel → CheckLevelUp(client) / CheckLevelUpServer(server)
     ▼
IGameServer → MercenaryPVO / UserPVO 반영
```

---

# Shop / Gacha

### Shop

* `ShopItemDefinition`(진열 단위) → `ProductDefinition`(상품) → `RewardBase`(보상: Currency / Mercenary)
* 구매 타입: Free / Currency / Ad / IAP, 제한 타입: None / Daily / Lifetime
* 상품 PVO 변경 이벤트로 UI 개별 갱신, 중복 구매 요청 방지(`_isPurchasing`)

### Gacha

* `GachaDefinition`의 가중치(weight) 기반 추첨, 1회 / 10회 뽑기
* 재화 부족, 잘못된 뽑기 횟수, 보상 데이터 검증 및 실패 시 재화 환불 처리
* 뽑기 결과 팝업에서 연속 뽑기 지원
* 에디터 확장: 가중치 → 확률(%) 자동 계산 및 합계 검증 (`GachaDefinitionEditor`)

---

# Addressables

Addressables를 이용하여 런타임 리소스를 관리합니다.

### 주요 내용

* `IAssetService` / `AssetLoadingTask` 기반 비동기 에셋 로딩 및 해제
* `Preload` 라벨 리소스를 `PreloadCache`로 세션 동안 상주 관리
* 데이터 테이블(`*DefinitionTable`)을 Addressables로 병렬 로드 (`GameDataLoader`)
* Popup / UI / Prefab / Material / Localization 동적 로딩
* 그룹 분리: Default Local / Local Game Data / Local Popup / Localization

```csharp
public void LoadPrefab(string address, Action<GameObject> onLoaded, Action onFailed = null)
```

---

# MVP + DI

UI는 View와 Presenter의 책임을 분리하고 `Deps` 객체를 통해 필요한 의존성만 전달합니다.

```text
UI
 ├── View       화면 표현 / 이벤트 노출 (Action, event)
 └── Presenter  게임 로직 / Service 호출 / View 갱신 (IDisposable)
```

### 주요 내용

* Presenter 중심의 UI 로직, View는 입력 이벤트와 표시만 담당
* Lobby: `UILobbyPresenter` → Top / Middle(Barrack, Battle, Shop, Gacha) Presenter 구성
* Battle: `UIBattlePresenter` → Top / Dash Presenter, HpBar 컨트롤러
* Popup: `UIPopup` + `UIPopupDependencies`로 서비스 주입, 개별 Presenter 연결
* Bind / Unbind 쌍으로 이벤트 해제 보장

---

# UI Framework

Canvas 기반 UI Framework를 직접 구현했습니다.

### 주요 기능

* `PopupService`: 주소 기반 동적 생성, 팝업 스택, 알파 마스크 페이드, 닫힘 애니메이션
* `InfiniteScrollBase` → `VerticalInfiniteScroll` / `HorizontalInfiniteScroll`
  * 아이템 풀링(가시 영역 + 여유분만 생성) 및 데이터 Provider / Item Factory 분리
  * 스냅, 중앙 아이템 확대(Center Scale), 인덱스 이동, 스크롤 정지 이벤트
* `UIHorizontalMenuFitter` (선택 메뉴 확대), `UITabGroup`, `UILongPressButton`, `UIDrag`
* 로컬라이즈 연동 (`LocalizationHelper` 캐시, 로케일 변경 시 UI 갱신)
* 설정 팝업: 언어 / BGM / SFX 볼륨 (`DeviceSettings` 지연 저장)

---

# Combat System

전투 시스템은 Role(데이터)과 Actor(런타임 객체)를 분리하여 설계했습니다.

```text
Role (RoleDefinition → Role<TData>)
 ├── Mercenary
 ├── Monster
 ├── Projectile
 └── DropItem (Element / Gold)

Actor (ActorBase → Actor<TRole>)
 ├── Player
 ├── Enemy
 ├── ActorProjectile
 └── ActorDIElement / ActorDIGold
```

### 전투 흐름

```text
BattleScene
 └── BattleSession
      ├── BattleFlow      Ready → Running → Win / Lose
      ├── StageRunner     스테이지 클리어 조건(Runtime) 실행
      └── ActorWorld
           ├── ActorSpawner      풀링 / ID / 렌더 순서
           ├── EnemySpawner      가중치 기반 몬스터 스폰
           ├── DropItemSpawner   가중치 기반 드롭 아이템
           └── MapLevelManager   시간 / 처치 수 기반 맵 레벨 진행
```

### 주요 내용

* `ActorStateModel` 상태 전이 테이블 (Idle / Move / Dash / Die)
* `IMoveBehaviour` 전략 패턴: Straight / Orbit / Homing / GroundZone / ChaseTarget / Direction
* 스킬 (`Skill`, `Skill01~03`, `MultiShotSkill`): 연사 / 다중 발사 / 궤도 / 소멸 시 추가 발사체
* `DashAbility`: 횟수 / 쿨타임, 대시 중 충돌 해제, 셰이더 연출
* 속성(Water / Nature / Fire) 수집 → 레벨업 시 이동속도·대시 / 최대 체력 / 스킬 강화
* `DamageCalculator`: 공격력 × 배율, 치명타, 방어력 감쇠
* 스테이지 클리어 조건을 `StageClearCondition`(SO) → `Runtime` 구조로 분리 (예: 처치 수)
* 카메라 맵 경계 클램프 / 줌아웃, 화면 밖 액터 애니메이션·HP바 비활성화(`RenderCheck`, Frustum)

---

# Object Pooling

전투 중 반복적으로 생성되는 객체를 Pooling하여 `Instantiate` 및 GC 발생을 줄입니다.

| 대상 | 구현 |
| --- | --- |
| Projectile / Monster / DropItem | `ActorSpawner` (Role ID별 Queue 풀, 부족 시 확장) |
| HP Bar | `HpBarController` (풀 + 대기 큐) |
| Scroll Item | `InfiniteScrollBase` (가시 영역 기반 재사용) |
| 용병 미리보기 | `MercenaryViewPool` |
| WaitForSeconds | `WaitCache` |

---

# Data Driven Design

게임 데이터는 ScriptableObject를 기반으로 관리합니다.

```text
DefinitionBase (_id)
 ├── RoleDefinition → Mercenary / Monster / Projectile / DropItem
 ├── StageDefinition / MapDefinition / MapLevelDefinition / SpawnDefinition
 ├── ProductDefinition / ShopItemDefinition / RewardBase
 ├── GachaDefinition
 ├── SoundDefinition
 └── SkillDefinition / SkillTreeDefinition

IdSyncTable → *DefinitionTable (SerializableDictionary<int, T>)
Level / Grade Table → 에디터에서 구간 자동 생성
```

* `RoleFactory` / `*Catalog`가 Definition을 런타임 Role로 변환 (`DeepCopy`로 원본 보호)
* `IdSyncTableEditor`: 테이블 키를 Definition ID와 일괄 동기화
* `UserLevelTableEditor` / `MercenaryLevelTableEditor` / `MercenaryGradeTableEditor`: 테이블 생성 버튼
* `TMPFontChangerWindow`: 씬 / 프리팹 TMP 폰트 일괄 교체

---

# Localization

* Unity Localization 기반 ko / en 지원 (String Table: Common, Mercenary, Monster, Projectile, Shop, DropItem, Skill, Stat, ErrorCode)
* Definition ID 기반 키 생성 (`name_{id}`, `desc_{id}`)
* SmartFormat 인자 지원 (구매 결과 메시지 등)
* 서버 결과 코드를 에러 코드 테이블 키로 매핑

---

# Debug

개발 중 기능 테스트를 빠르게 수행할 수 있도록 Debug 도구를 구현했습니다.

```text
Debug Console (드래그 가능한 원형 메뉴)
 ├── Add User EXP
 └── Add Gold
Battle Debug Input
 ├── P : 적 일시정지
 └── O : 스테이지 즉시 클리어
```

Debug 기능은 `DEBUG_MODE` 조건부 컴파일로 실제 빌드에서 제외할 수 있도록 구성했습니다.

---

# Shader

ShaderLab / HLSL을 이용해 게임에 필요한 커스텀 셰이더와 이펙트를 구현했습니다.

* `Flash`: 피격 시 색상 플래시 (MaterialPropertyBlock)
* `Dash`: 방향 기반 블러 + 알파 페이드 대시 잔상
* `Map`: 월드 좌표 기반 타일링 맵 텍스처
* `LobbyBackground`: `Graphics.RenderSprite` 기반 다중 레이어 패럴랙스 스크롤
* URP 환경 대응

---

# Project Structure

```text
Assets
 ├── Script
 │    ├── Core            AppRoot / Context(App, Game, Battle) / Session / Service / Factory / Loader
 │    ├── Definitions     ScriptableObject 정의 / Table / System(Level, Grade)
 │    ├── Role            Role / RoleStat / 전투력 계산
 │    ├── Actor           Actor / Controller / Move / Skill / View / DropItem / Input
 │    ├── Battle          카메라 Frustum 등
 │    ├── Map             Map / MapLevel
 │    ├── Scene           Boot / Lobby / Battle Scene
 │    ├── UI              Boot / Lobby / Battle / Popup / Shop / Components / Debug
 │    ├── Server_temp     IGameServer / Protocol / Mock (Handler, Domain, Storage)
 │    ├── Shader
 │    └── Utils
 ├── Editor               커스텀 에디터 / 도구
 ├── Resource             데이터 / Material / 프리팹
 ├── Localization         String Table / Locale
 ├── AddressableAssetsData
 └── Scenes
```

---

# Development Focus

* 수명(App / Game / Battle)별 Context 분리와 명시적 의존성 주입
* 서버 / 클라이언트 역할 분리 (Handler / Domain, Request / Response / PVO)
* 클라이언트 / 서버가 공유하는 규칙 시스템 분리 (Level / Grade System)
* 데이터와 게임 로직 분리 (ScriptableObject Definition → Runtime Role)
* 유지보수 가능한 MVP UI 구조와 이벤트 Bind / Unbind 관리
* 확장 가능한 전투 시스템 (Role / Actor 분리, 이동·스킬·클리어 조건 전략화)
* Addressables 기반 리소스 관리 및 단계별 부팅
* Object Pooling / 화면 밖 최적화를 통한 런타임 비용 절감
* 에디터 툴을 통한 데이터 작업 효율화
* 모바일 환경을 고려한 UI 및 그래픽 구현

---

# Run

## Environment

* Unity 6000.6.3f1
* Android Build Support

## Execute

1. 프로젝트 Clone
2. Unity 6000.6.3f1 로 프로젝트 실행
3. Android Platform으로 변경
4. Portrait 환경 설정
5. `01.BootScene`에서 Play

> 저장 데이터를 초기화하려면 에디터 메뉴 `CustomMenu/DataDelete*`를 사용합니다.
> 디버그 기능은 Scripting Define Symbols에 `DEBUG_MODE`를 추가하면 활성화됩니다.

---

# Repository

[GitHub Repository](https://github.com/jhohsjob/portfolio)

---

# Author

**구조 설계와 유지보수성**을 중요하게 생각하며 개발하고 있습니다.

### Interests

* Unity
* C#
* Mobile Game Client
* UI Architecture
* Gameplay System
* Client / Server Architecture
* Shader
