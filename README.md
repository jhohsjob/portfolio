# Portfolio

Unity 6 기반으로 제작한 개인 게임 클라이언트 포트폴리오 프로젝트입니다.

단순한 기능 구현보다 **구조 설계, 의존성 관리, 유지보수성, 확장성**에 중점을 두고 개발하고 있습니다.

실제 게임 프로젝트에서 발생할 수 있는 요구사항을 가정하여 **DI 기반 클라이언트 구조, MVP 패턴, Addressables, ScriptableObject 데이터 설계, 서버-클라이언트 구조, Object Pooling, UI Framework, 전투 시스템** 등을 구현했습니다.

---

# Preview

* Unity 6 기반 모바일 게임 클라이언트
* Android Portrait 환경
* 모바일 UI 및 전투 시스템
* 용병 획득 및 성장 시스템
* MockGameServer 기반 서버-클라이언트 구조
* Addressables 기반 리소스 관리
* DI + MVP 기반 UI 구조
* Object Pooling
* ShaderLab / HLSL 기반 커스텀 셰이더

---

# Tech Stack

| Category     | Stack                    |
| ------------ | ------------------------ |
| Engine       | Unity 6000.6.3f1                  |
| Language     | C#                       |
| Rendering    | URP / ShaderLab / HLSL   |
| Platform     | Android                  |
| Architecture | DI / MVP / MVC           |
| Data         | ScriptableObject / PVO   |
| Asset System | Addressables             |
| Async        | UniTask                  |
| UI           | Unity UGUI / TextMeshPro |
| Animation    | DOTween / Spine          |

---

# Architecture

## DI 기반 클라이언트 구조

기존 Singleton 중심의 접근에서 벗어나 필요한 의존성을 `GameContext`를 통해 관리하도록 구성했습니다.

```text
GameContext
 ├── IAssetService
 ├── IAssetLoader
 ├── IPopupService
 ├── ISceneLoader
 ├── ICurrencyService
 └── IGameServer
```

이를 통해 UI나 게임 로직에서 특정 Singleton에 직접 의존하는 것을 줄이고, 필요한 서비스를 명시적으로 전달할 수 있도록 구성했습니다.

### 주요 내용

* GameContext 기반 의존성 관리
* 인터페이스를 통한 서비스 추상화
* Singleton 의존성 최소화
* UI Context를 통한 Presenter 의존성 주입
* 기능별 서비스 분리

---

# Server / Client

실제 서버가 없는 개발 환경에서도 서버-클라이언트 구조를 유지할 수 있도록 `MockGameServer`를 구현했습니다.

```text
Client
  │
  ▼
IGameServer
  │
  ▼
MockGameServer
  │
  ├── MockUserService
  ├── MockMercenaryService
  └── MockPurchaseService
```

클라이언트가 로컬 데이터를 직접 수정하는 방식 대신 서버 요청을 거치는 구조로 구현하여, 이후 실제 네트워크 서버로 교체할 수 있도록 구성했습니다.

### 주요 기능

* 서버 요청 / 응답 구조
* Response 기반 PVO 갱신
* User / Mercenary / Product 데이터 분리
* 서비스 단위 기능 분리
* 서버 결과 코드 기반 예외 처리

---

# Player Data

게임 데이터를 런타임 객체와 분리하기 위해 PVO 구조를 사용했습니다.

```text
InitializeResponse
 ├── UserPVO
 ├── MercenaryPVO
 └── ProductPVO
```

서버 응답으로 전달받은 데이터를 클라이언트 상태에 반영하는 형태로 구성했습니다.

이를 통해 향후 실제 서버 API를 적용하더라도 클라이언트의 데이터 처리 구조를 크게 변경하지 않는 것을 목표로 했습니다.

---

# Mercenary System

용병 데이터를 `ScriptableObject` 기반 Definition과 런타임 PVO로 분리했습니다.

```text
MercenaryDefinition
        │
        ▼
    Mercenary
        │
        ▼
   MercenaryPVO
```

### 구현 기능

* 용병 목록 및 데이터 관리
* 용병 획득
* 보유 여부 관리
* 용병 레벨업
* 최대 레벨 제한
* 레벨업 비용 계산
* 골드 부족 및 유효하지 않은 레벨 처리
* 서버 요청 후 PVO 반영

### Level System

용병 레벨에 필요한 규칙을 `MercenaryLevelSystem`으로 분리했습니다.

```text
User Level
     │
     ├── Level Limit
     │
     ▼
Maximum Mercenary Level

Current Level
     │
     ▼
Level Up Cost
     │
     ▼
IGameServer
     │
     ▼
MercenaryPVO Update
```

레벨업 가능 여부와 비용 계산을 UI에서 직접 처리하지 않고 별도의 시스템에서 관리하도록 구성했습니다.

---

# Addressables

Addressables를 이용하여 런타임 리소스를 관리합니다.

### 주요 내용

* 비동기 에셋 로딩
* Preload 리소스 관리
* Label 기반 리소스 로딩
* UI / Prefab / Effect 동적 로딩
* Localization 초기화
* 런타임 메모리 관리

```csharp
public async UniTask<T> LoadAsset<T>(string key)
{
    var handle = Addressables.LoadAssetAsync<T>(key);
    await handle.Task;

    return handle.Result;
}
```

초기화 과정도 단계별로 분리하여 Addressables, Localization, Preload Asset 등의 진행 상태를 관리할 수 있도록 구성했습니다.

---

# MVP + DI

UI는 View와 Presenter의 책임을 분리하고 Context를 통해 필요한 의존성을 전달합니다.

```text
UI
 │
 ├── View
 │     └── 화면 표현
 │
 └── Presenter
       ├── 게임 로직
       ├── Service 호출
       └── View 갱신
```

### 주요 내용

* Presenter 중심의 UI 로직
* View는 화면 표현 담당
* Context 기반 의존성 주입
* Popup Service 추상화
* Scene Loader 추상화
* Service 인터페이스 기반 설계

---

# UI Framework

Canvas 기반 UI Framework를 직접 구현했습니다.

### 주요 기능

* UI Layer 관리
* Popup Stack
* Popup Service
* Top / Middle / Popup UI 분리
* 동적 UI 생성
* 공통 Presenter 구조
* Drag / Scroll UI
* Infinite Scroll
* Debug UI

Popup은 주소와 데이터를 기반으로 동적으로 생성할 수 있도록 구성했습니다.

```csharp
_popupService.ShowPopup<T>(
    address,
    data,
    callback
);
```

---

# Debug UI

개발 중 기능 테스트를 빠르게 수행할 수 있도록 Debug UI를 구현했습니다.

```text
Debug Console
 ├── Add User EXP
 ├── Add Gold
 └── Game Data Test
```

Debug 기능은 `DEBUG_MODE` 조건부 컴파일을 통해 실제 빌드에서 제외할 수 있도록 구성했습니다.

---

# Combat System

전투 시스템은 Actor와 Role을 분리하여 설계했습니다.

```text
Role
 ├── Mercenary
 ├── Monster
 └── Projectile

Actor
 ├── ActorBase
 └── Actor<TRole>
```

### 주요 내용

* Actor 상태 관리
* 이동
* 방향 전환
* Dash
* 충돌 처리
* Target 처리
* Projectile
* Skill
* 모바일 Joystick 입력
* HP / Stat 관리

전투 중 생성되는 Projectile과 Effect는 Object Pool을 활용하여 재사용합니다.

---

# Object Pooling

전투 중 반복적으로 생성되는 객체를 Pooling하여 `Instantiate` 및 GC 발생을 줄이는 구조를 구현했습니다.

### Pool 대상

* Projectile
* Effect
* Monster
* 기타 반복 생성 객체

```csharp
public T Pop<T>() where T : Poolable
{
    return _pool.Pop() as T;
}
```

---

# Data Driven Design

게임 데이터는 ScriptableObject를 기반으로 관리합니다.

```text
Definition
 ├── StageDefinition
 ├── MercenaryDefinition
 ├── ProductDefinition
 └── Skill / Reward Data
```

게임 로직과 데이터 정의를 분리하여 코드 수정 없이 콘텐츠 데이터를 변경할 수 있는 구조를 목표로 했습니다.

---

# Shader

ShaderLab / HLSL을 이용해 게임에 필요한 커스텀 셰이더와 이펙트를 구현했습니다.

### 주요 구현

* 캐릭터 Aura Effect
* 속성별 이펙트
* Dash 연출
* Sprite 기반 효과
* UI Effect
* URP 환경 대응

---

# Project Structure

```text
Assets
 ├── Runtime
 │    ├── Actor
 │    ├── Battle
 │    ├── UI
 │    ├── Service
 │    ├── Server
 │    ├── Data
 │    ├── Pool
 │    └── Shader
 │
 ├── Addressables
 ├── ScriptableObjects
 └── Scenes
```

---

# Development Focus

이 프로젝트에서는 다음과 같은 부분을 중점적으로 개발했습니다.

* 기능 간 의존성 감소
* DI 기반 구조 설계
* 서버 / 클라이언트 역할 분리
* 데이터와 게임 로직 분리
* 유지보수 가능한 UI 구조
* 확장 가능한 전투 시스템
* Addressables 기반 리소스 관리
* Object Pooling을 통한 런타임 최적화
* ScriptableObject 기반 데이터 설계
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
5. Play

---

# Repository

[GitHub Repository](https://github.com/jhohsjob/portfolio)

---

# Author

게임 클라이언트 개발자를 목표로 **구조 설계와 유지보수성**을 중요하게 생각하며 개발하고 있습니다.

### Interests

* Unity
* C#
* Mobile Game Client
* UI Architecture
* Gameplay System
* Client / Server Architecture
* Shader
