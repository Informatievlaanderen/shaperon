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
