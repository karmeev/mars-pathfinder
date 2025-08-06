VERSION := $(shell cat $(ROOT_DIR)/VERSION.txt)
APP_TEST=cd/stage

UNIT_TESTS=./src/nasa-server/tests
INTEGRATION_TESTS=./tests/integration

UNIT_TEST_PROJECTS := \
    Nasa.Pathfinder.Facades.Tests/Nasa.Pathfinder.Facades.Tests.csproj \
    Nasa.Pathfinder.Services.Tests/Nasa.Pathfinder.Services.Tests.csproj \
    
INTEGRATION_TEST_PROJECTS := \
    Nasa.IntegrationTests.Pathfinder/Nasa.IntegrationTests.Pathfinder.csproj \

coverage:
	@for proj in $(UNIT_TEST_PROJECTS); do \
		echo "Running coverage for $$proj..."; \
		dotnet test $(UNIT_TESTS)/$$proj \
			--configuration Release \
			--collect:"XPlat Code Coverage" \
			--results-directory ./TestResults \
			--logger "trx;LogFileName=test-results.trx"; \
	done

	reportgenerator \
		-reports:./TestResults/**/coverage.cobertura.xml \
		-targetdir:./TestResults/CoverageReport \
		-reporttypes:MarkdownSummaryGithub \
		"-assemblyfilters:+Nasa.*;-Nasa.*.Contracts;-Nasa.Domain;-Nasa.*.Tests;-xunit*;-System.*;-Microsoft.*"

test:
	@if [ "$(CATEGORY)" = "Unit" ]; then \
		TEST_PATH=$(UNIT_TESTS); \
		PROJECTS="$(UNIT_TEST_PROJECTS)"; \
	elif [ "$(CATEGORY)" = "Integration" ]; then \
		TEST_PATH=$(INTEGRATION_TESTS); \
		PROJECTS="$(INTEGRATION_TEST_PROJECTS)"; \
	else \
		echo "Unsupported CATEGORY '$(CATEGORY)'. Use 'Unit' or 'Integration'."; \
		exit 1; \
	fi; \
	for proj in $$PROJECTS; do \
		echo "Running $(CATEGORY) tests in $$proj..."; \
		dotnet test $$TEST_PATH/$$proj \
			--configuration Release \
			--no-restore \
			--logger "console;verbosity=detailed" \
			--filter "Category=$(CATEGORY)"; \
	done

integration_tests_up:
	echo "Using IMAGE_TAG: $(IMAGE_TAG)"
	echo "IMAGE_TAG=$(IMAGE_TAG)" > .env
	echo "APP_VERSION=$(cat VERSION.txt)" >> .env
	docker compose -f ${APP_TEST}/docker-compose.yaml up -d

integration_tests_down:
	docker compose -f ${APP_TEST}/docker-compose.yaml down