UNIT_TESTS=./src/nasa-server/tests

UNIT_TEST_PROJECTS := \
    Nasa.Pathfinder.Facades.Tests/Nasa.Pathfinder.Facades.Tests.csproj \

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
		-reporttypes:MarkdownSummaryGithub
		-assemblyfilters:+Currency.*;-*.Contracts;-*Tests;-xunit*;-System.*;-Microsoft.*