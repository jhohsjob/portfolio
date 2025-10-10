﻿# Portfolio Project

Unity6 기반 개인 포트폴리오 프로젝트입니다.  
최신 Unity 엔진 기능을 활용하여 구조적 설계, Addressables, 셰이더, UI 시스템 등을 직접 구현한 샘플 프로젝트입니다.

## 프로젝트 개요

엔진 : Unity 6000.xx
언어 : C#
빌드 : PC

## 주요 기능

- Addressables 시스템
  - 비동기 로드 및 메모리 최적화
- ScriptableObject 시스템
  - 데이터 구조화
- Object Pooling
  - 런타임 오브젝트 재사용으로 GC 최소화
- 전투 / 이동 시스템
  - 입력 처리, Actor 관리, 충돌 제어
- UI 팝업 프레임워크
  - Canvas 기반 다중 팝업 관리 및 정렬
- 씬 전환 및 초기화 흐름
  - `Launch → Battle` 구조로 관리