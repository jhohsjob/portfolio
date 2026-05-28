# Portfolio

Unity 6 기반으로 제작한 개인 포트폴리오 프로젝트입니다.  
단순 기능 구현보다 **구조 설계**, **유지보수성**, **확장성**에 중점을 두고 개발했습니다.

프로젝트 전반에 Addressables, DI 기반 구조, MVP 패턴, Object Pooling, ScriptableObject 데이터 설계 등을 적용하여 실제 게임 프로젝트에서 사용할 수 있는 형태를 목표로 구성했습니다.

\---

# Preview

* Android Portrait 환경 기준 제작
* Unity 6000.4.8f1 사용
* 모바일 UI 및 전투 흐름 구현
* 커스텀 셰이더 및 이펙트 적용

\---

# Tech Stack

|Category|Stack|
|-|-|
|Engine|Unity 6 (6000.3.0f1)|
|Language|C#|
|Rendering|ShaderLab / HLSL|
|Platform|Android|
|Architecture|MVP, DI|
|Data|ScriptableObject|
|Asset System|Addressables|

\---

# Core Features

## Addressables 기반 리소스 관리

* 비동기 에셋 로딩
* 런타임 메모리 관리
* 리소스 의존성 분리
* UI / 이펙트 / 프리팹 동적 로드

```csharp
public async UniTask<T> LoadAsset<T>(string key)
{
    var handle = Addressables.LoadAssetAsync<T>(key);
    await handle.Task;
    return handle.Result;
}
```

\---

## MVP + DI 구조 설계

UI 로직과 View 를 분리하여 테스트 및 유지보수가 가능하도록 구성했습니다.

* Presenter 중심 로직 처리
* View 는 UI 표현만 담당
* Context 기반 의존성 주입
* Singleton 의존 최소화

```csharp
public class UILobbyContext
{
    public IAssetLoader assetLoader;
    public IPopupService popupService;
    public ISceneLoader sceneLoader;
}
```

\---

## Object Pooling

전투 중 생성되는 오브젝트의 GC 발생을 줄이기 위해 Object Pooling 시스템을 구현했습니다.

* 이펙트 재사용
* 몬스터 / 투사체 재사용
* 런타임 Instantiate 최소화

```csharp
public T Pop<T>() where T : Poolable
{
    return \_pool.Pop() as T;
}
```

\---

## 전투 및 이동 시스템

플레이어 입력과 Actor 상태를 분리하여 관리합니다.

* 이동 입력 처리
* Dash 시스템
* Actor State 관리
* 충돌 및 타겟 처리
* 모바일 조이스틱 입력

```csharp
\_state.SetState(\_inputSource.MoveDirection == Vector2.zero
    ? ActorState.Idle
    : ActorState.Move);
```

\---

## UI Framework

Canvas 기반 UI 시스템을 직접 구현했습니다.

* Popup Stack 관리
* UI Layer 분리
* 공통 Presenter 구조
* Top / Middle / Popup UI 관리
* 동적 UI 생성

\---

## Shader

ShaderLab + HLSL 기반 셰이더를 제작했습니다.

* 캐릭터 이펙트
* Dash 연출
* UI 이펙트
* 모바일 환경 고려

\---

# Project Structure

```text
Assets
 ┣ Runtime
 ┃ ┣ Actor
 ┃ ┣ Battle
 ┃ ┣ UI
 ┃ ┣ Resource
 ┃ ┣ Pool
 ┃ ┗ Shader
 ┣ Addressables
 ┣ ScriptableObjects
 ┗ Scenes
```

\---

# Focused On

이 프로젝트에서는 아래 내용을 중점적으로 작업했습니다.

* 유지보수 가능한 구조 설계
* 기능 간 의존성 감소
* 모바일 환경 최적화
* 재사용 가능한 UI 구조
* 런타임 메모리 관리
* 확장 가능한 전투 시스템

\---

# Run

## Environment

* Unity 6000.3.0f1
* Android Build Support

## Execute

1. 프로젝트 Clone
2. Unity 6000.3.0f1 로 실행
3. Android Platform 으로 변경
4. Portrait 해상도 설정
5. Play

\---

# Repository

[GitHub Repository](https://github.com/jhohsjob/portfolio?utm_source=chatgpt.com)

\---

# Author

게임 클라이언트 개발자를 목표로 구조 설계와 유지보수성을 중요하게 생각하며 개발하고 있습니다.

* Unity
* C#
* Mobile Game Client
* UI Architecture
* Gameplay System
* Shader

