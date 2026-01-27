.PHONY: help create-establishment reset-db

.DEFAULT_GOAL := help

help: ## Show this help message
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

create-establishment: ## Create a new establishment
	dotnet run -- establishment add

reset-db: ## Reset database and recreate migrations
	rm -rf ./Migrations
	dotnet ef database drop --force
	dotnet ef migrations add InitialCreate
