# [7.3.0](https://github.com/informatievlaanderen/shaperon/compare/v7.2.0...v7.3.0) (2020-01-31)


### Features

* upgrade netcoreapp31 and dependencies ([f3ababc](https://github.com/informatievlaanderen/shaperon/commit/f3ababcd2f3cf657991d923f67d09a760a2aeee3))

# [7.2.0](https://github.com/informatievlaanderen/shaperon/compare/v7.1.0...v7.2.0) (2019-12-16)


### Features

* improve dbase float and number functions ([2cc2378](https://github.com/informatievlaanderen/shaperon/commit/2cc2378f768834c40c29890d84edd619adb4e9e4))
* move back to dbase primitives ([86a7e43](https://github.com/informatievlaanderen/shaperon/commit/86a7e433878b31884a72865e471fb7ccb3a37343))
* remove legacy and improve dbasecharacter ([bf5ea9d](https://github.com/informatievlaanderen/shaperon/commit/bf5ea9da92841afcc5e813cf4281aa56d450170a))
* upgrade to netcoreapp31 ([77affc7](https://github.com/informatievlaanderen/shaperon/commit/77affc7ebc99be97577f1088448d7d80d36b4f8d))

# [7.1.0](https://github.com/informatievlaanderen/shaperon/compare/v7.0.0...v7.1.0) (2019-12-09)


### Features

* relax dbase file header reading ([ae86572](https://github.com/informatievlaanderen/shaperon/commit/ae86572))

# [7.0.0](https://github.com/informatievlaanderen/shaperon/compare/v6.0.1...v7.0.0) (2019-11-22)


### Code Refactoring

* upgrade to netcoreapp30 ([80f1220](https://github.com/informatievlaanderen/shaperon/commit/80f1220))


### BREAKING CHANGES

* Upgrade to .NET Core 3

## [6.0.1](https://github.com/informatievlaanderen/shaperon/compare/v6.0.0...v6.0.1) (2019-10-28)


### Bug Fixes

* upgrade build tools, move to fake 5 ([564df97](https://github.com/informatievlaanderen/shaperon/commit/564df97))

# [6.0.0](https://github.com/informatievlaanderen/shaperon/compare/v5.0.0...v6.0.0) (2019-09-18)


### Bug Fixes

* integrated with nts 2 dot 0 type system ([6e8672c](https://github.com/informatievlaanderen/shaperon/commit/6e8672c))


### Features

* force build for nts 2.x ([8676de7](https://github.com/informatievlaanderen/shaperon/commit/8676de7))


### BREAKING CHANGES

* removed pointm and pointsequence

# [5.0.0](https://github.com/informatievlaanderen/shaperon/compare/v4.2.0...v5.0.0) (2019-08-26)


### Features

* added tolerance as type and to signatures ([bbfa892](https://github.com/informatievlaanderen/shaperon/commit/bbfa892))
* nts integration as package ([bb01268](https://github.com/informatievlaanderen/shaperon/commit/bb01268))
* reintroduced nts geometry under test ([0a45d6e](https://github.com/informatievlaanderen/shaperon/commit/0a45d6e))
* removed nettopologysuite as dependency ([8e381d1](https://github.com/informatievlaanderen/shaperon/commit/8e381d1))


### BREAKING CHANGES

* no longer depend on nts, api changes around shape content

test: more coverage

test: more tests

feat: obsolete anonymous, to and from bytes
* - obsolete anonymous, to and from bytes

test: extra test

# [4.2.0](https://github.com/informatievlaanderen/shaperon/compare/v4.1.1...v4.2.0) (2019-08-22)


### Features

* bump to .net 2.2.6 ([88a0efa](https://github.com/informatievlaanderen/shaperon/commit/88a0efa))

## [4.1.1](https://github.com/informatievlaanderen/shaperon/compare/v4.1.0...v4.1.1) (2019-04-26)

# [4.1.0](https://github.com/informatievlaanderen/shaperon/compare/v4.0.1...v4.1.0) (2019-04-16)


### Features

* add PolygonShapeContent ([4b01f9a](https://github.com/informatievlaanderen/shaperon/commit/4b01f9a))

## [4.0.1](https://github.com/informatievlaanderen/shaperon/compare/v4.0.0...v4.0.1) (2019-03-01)


### Bug Fixes

* slight change to how record number progresses ([ad60a64](https://github.com/informatievlaanderen/shaperon/commit/ad60a64))

# [4.0.0](https://github.com/informatievlaanderen/shaperon/compare/v3.1.0...v4.0.0) (2019-02-28)


### Features

* dbase record enumerators ([db7da4a](https://github.com/informatievlaanderen/shaperon/commit/db7da4a))


### BREAKING CHANGES

* removed numbered dbase record support

# [3.1.0](https://github.com/informatievlaanderen/shaperon/compare/v3.0.1...v3.1.0) (2019-02-28)


### Features

* enumerators for shape and dbase records ([c894472](https://github.com/informatievlaanderen/shaperon/commit/c894472))

## [3.0.1](https://github.com/informatievlaanderen/shaperon/compare/v3.0.0...v3.0.1) (2019-02-26)

# [3.0.0](https://github.com/informatievlaanderen/shaperon/compare/v2.0.0...v3.0.0) (2019-02-16)


### Features

* switch dbaserecordexception for endofstreamexception ([1431323](https://github.com/informatievlaanderen/shaperon/commit/1431323))


### BREAKING CHANGES

* different exception being thrown by dbaserecord.read()

# [2.0.0](https://github.com/informatievlaanderen/shaperon/compare/v1.2.0...v2.0.0) (2019-02-13)


### Features

* change encoding resolution ([79b9880](https://github.com/informatievlaanderen/shaperon/commit/79b9880))
* dbase field name ignores casing ([f3ac0eb](https://github.com/informatievlaanderen/shaperon/commit/f3ac0eb))
* make each exception take an exception as a parameter ([ce93434](https://github.com/informatievlaanderen/shaperon/commit/ce93434))
* split dbasefileheader and dbaserecord exception ([0e02474](https://github.com/informatievlaanderen/shaperon/commit/0e02474))


### BREAKING CHANGES

* remove CodePage property, ToEncoding takes an EncodingProvider
* dbaserecord reading throw different exception
* casing is being ignored for dbase field names

# [1.2.0](https://github.com/informatievlaanderen/shaperon/compare/v1.1.0...v1.2.0) (2019-01-08)


### Features

* add ToEncoding for DbaseCodePage ([163c57c](https://github.com/informatievlaanderen/shaperon/commit/163c57c))

# [1.1.0](https://github.com/informatievlaanderen/shaperon/compare/v1.0.2...v1.1.0) (2019-01-08)


### Bug Fixes

* CodePage property is now int? instead of int ([16df603](https://github.com/informatievlaanderen/shaperon/commit/16df603))


### Features

* add CodePage property for DbaseCodePage ([bfcb6a8](https://github.com/informatievlaanderen/shaperon/commit/bfcb6a8))

## [1.0.2](https://github.com/informatievlaanderen/shaperon/compare/v1.0.1...v1.0.2) (2018-12-18)

## [1.0.1](https://github.com/informatievlaanderen/shaperon/compare/v1.0.0...v1.0.1) (2018-12-18)

# 1.0.0 (2018-12-18)


### Features

* open source with MIT license as 'agentschap Informatie Vlaanderen' ([629739b](https://github.com/informatievlaanderen/shaperon/commit/629739b))
