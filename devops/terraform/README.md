# Terraform - Infrastructure as Code

Terraform configurations for Naar-Noor infrastructure provisioning across AWS, Azure, and GCP.

## Directory Structure

```
terraform/
├── main.tf                  # Main Terraform configuration
├── variables.tf             # Input variables
├── outputs.tf               # Output values
├── providers.tf             # Provider configuration
├── backends.tf              # State backend configuration
├── locals.tf                # Local values
├── terraform.tfvars.example # Example variables file
├── environments/            # Environment-specific variables
│   ├── dev.tfvars
│   ├── staging.tfvars
│   └── production.tfvars
├── modules/                 # Reusable Terraform modules
│   ├── kubernetes/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── outputs.tf
│   │   └── README.md
│   ├── database/
│   │   └── ...
│   ├── networking/
│   │   └── ...
│   └── storage/
│       └── ...
├── aws/                     # AWS-specific resources
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
├── azure/                   # Azure-specific resources
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
├── gcp/                     # GCP-specific resources
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
└── README.md
```

## Prerequisites

### Installation
```bash
# macOS
brew install terraform

# Linux
wget https://releases.hashicorp.com/terraform/1.6.0/terraform_1.6.0_linux_amd64.zip
unzip terraform_1.6.0_linux_amd64.zip
sudo mv terraform /usr/local/bin/

# Windows
choco install terraform
```

### Cloud Provider Credentials

#### AWS
```bash
aws configure
# Or set environment variables:
export AWS_ACCESS_KEY_ID="your-access-key"
export AWS_SECRET_ACCESS_KEY="your-secret-key"
export AWS_DEFAULT_REGION="us-east-1"
```

#### Azure
```bash
az login
# Or set environment variables:
export ARM_SUBSCRIPTION_ID="subscription-id"
export ARM_CLIENT_ID="client-id"
export ARM_CLIENT_SECRET="client-secret"
export ARM_TENANT_ID="tenant-id"
```

#### GCP
```bash
gcloud auth application-default login
# Or set service account key:
export GOOGLE_APPLICATION_CREDENTIALS="/path/to/service-account-key.json"
```

## Quick Start

### Initialize Terraform

```bash
cd devops/terraform

# Download provider plugins
terraform init

# Optionally configure remote state backend
# Edit backends.tf to enable Terraform Cloud, S3, Azure Blob, or GCS
terraform init -migrate-state
```

### Plan Infrastructure

```bash
# Review changes (dev environment)
terraform plan -var-file="environments/dev.tfvars"

# Review changes (production)
terraform plan -var-file="environments/production.tfvars" -out=tfplan
```

### Apply Infrastructure

```bash
# Apply changes (dev)
terraform apply -var-file="environments/dev.tfvars"

# Apply changes (production - requires review)
terraform apply tfplan
```

### Destroy Infrastructure

```bash
# WARNING: This is destructive!
terraform destroy -var-file="environments/production.tfvars"
```

## Configuration Files

### providers.tf
Defines cloud providers and versions

```hcl
terraform {
  required_version = ">= 1.0"
  
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    google = {
      source  = "hashicorp/google"
      version = "~> 5.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.23"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
}

provider "google" {
  project = var.gcp_project
  region  = var.gcp_region
}

provider "kubernetes" {
  host                   = data.aws_eks_cluster.cluster.endpoint
  cluster_ca_certificate = base64decode(data.aws_eks_cluster.cluster.certificate_authority.0.data)
  token                  = data.aws_eks_cluster_auth.cluster.token
}
```

### variables.tf
Input variables for configuration

```hcl
variable "environment" {
  type        = string
  description = "Environment (dev, staging, production)"
  validation {
    condition     = contains(["dev", "staging", "production"], var.environment)
    error_message = "Environment must be dev, staging, or production."
  }
}

variable "project_name" {
  type    = string
  default = "naar-noor"
}

variable "aws_region" {
  type    = string
  default = "us-east-1"
}

variable "kubernetes_version" {
  type    = string
  default = "1.27"
}

variable "database_password" {
  type        = string
  sensitive   = true
  description = "Database master password"
}
```

### backends.tf
State storage configuration

```hcl
terraform {
  # Local state (default - not recommended for teams)
  # backend "local" {
  #   path = "terraform.tfstate"
  # }

  # AWS S3 backend
  backend "s3" {
    bucket         = "naar-noor-terraform-state"
    key            = "prod/terraform.tfstate"
    region         = "us-east-1"
    encrypt        = true
    dynamodb_table = "naar-noor-tf-locks"
  }

  # Azure Blob Storage backend
  # backend "azurerm" {
  #   resource_group_name  = "terraform-rg"
  #   storage_account_name = "tfstate"
  #   container_name       = "tfstate"
  #   key                  = "prod.terraform.tfstate"
  # }

  # Google Cloud Storage backend
  # backend "gcs" {
  #   bucket = "naar-noor-terraform-state"
  #   prefix = "prod"
  # }

  # Terraform Cloud backend
  # backend "cloud" {
  #   organization = "naar-noor"
  #   workspaces {
  #     name = "production"
  #   }
  # }
}
```

### Environment Variables (environments/production.tfvars)

```hcl
environment           = "production"
aws_region            = "us-east-1"
kubernetes_version    = "1.27"
node_count            = 3
node_instance_type    = "t3.medium"
database_allocated_storage = 100
database_backup_retention_days = 30

# High availability
enable_multi_az = true
enable_read_replicas = true

# Security
enable_encryption = true
enable_monitoring = true
```

## Modules

Reusable Terraform modules for common infrastructure patterns.

### Kubernetes Module (`modules/kubernetes/`)

```hcl
module "kubernetes" {
  source = "./modules/kubernetes"

  cluster_name       = "naar-noor-${var.environment}"
  kubernetes_version = var.kubernetes_version
  node_count         = var.environment == "production" ? 3 : 1
  node_instance_type = var.node_instance_type

  tags = {
    Environment = var.environment
    Project     = var.project_name
  }
}
```

### Database Module (`modules/database/`)

```hcl
module "database" {
  source = "./modules/database"

  engine              = "postgres"
  engine_version      = "14.7"
  allocated_storage   = var.database_allocated_storage
  instance_class      = "db.t3.medium"
  backup_retention    = var.environment == "production" ? 30 : 7
  multi_az            = var.environment == "production"

  depends_on = [module.networking]
}
```

### Networking Module (`modules/networking/`)

```hcl
module "networking" {
  source = "./modules/networking"

  vpc_cidr = "10.0.0.0/16"
  
  subnets = {
    public  = ["10.0.1.0/24", "10.0.2.0/24"]
    private = ["10.0.10.0/24", "10.0.11.0/24"]
  }

  enable_nat_gateway = var.environment != "dev"
}
```

## Cloud Provider Selections

### AWS Deployment

```bash
terraform plan -var-file="environments/production.tfvars"

# Creates:
# - VPC with public/private subnets
# - EKS cluster
# - RDS PostgreSQL
# - ECR for container images
# - ALB/NLB for load balancing
# - CloudWatch for monitoring
```

### Azure Deployment

```bash
terraform plan -var-file="environments/production.tfvars"

# Creates:
# - Resource Group
# - AKS cluster
# - Azure Database for PostgreSQL
# - Container Registry (ACR)
# - Application Gateway for load balancing
# - Application Insights for monitoring
```

### GCP Deployment

```bash
terraform plan -var-file="environments/production.tfvars"

# Creates:
# - GKE cluster
# - Cloud SQL PostgreSQL
# - Artifact Registry for container images
# - Cloud Load Balancing
# - Cloud Monitoring
```

## Common Commands

### State Management

```bash
# View state
terraform state list
terraform state show module.kubernetes

# Backup state
cp terraform.tfstate terraform.tfstate.backup

# Unlock state (if locked)
terraform force-unlock <LOCK_ID>

# Import existing resource
terraform import aws_instance.example i-1234567890abcdef0
```

### Workspace Management

```bash
# List workspaces
terraform workspace list

# Create workspace
terraform workspace new staging

# Switch workspace
terraform workspace select production

# Delete workspace
terraform workspace delete dev
```

### Validation & Formatting

```bash
# Validate configuration
terraform validate

# Format code
terraform fmt -recursive

# Check compliance
terraform plan -json | jq '.resource_changes[] | select(.change.actions[] == "create")'
```

## Security Best Practices

✅ **Implemented:**
- Sensitive variables marked as sensitive
- State file encryption
- RBAC for service accounts
- Network policies
- Pod security policies
- Secrets management

✅ **Configured:**
- VPC isolation
- Private subnets for databases
- Security groups limiting access
- TLS encryption
- IAM roles with minimal permissions

⚠️ **Additional Recommendations:**
- Use Terraform Cloud for state locking
- Enable audit logging
- Implement secrets rotation
- Use HashiCorp Vault for secrets
- Enable MFA for cloud accounts

## Monitoring & Logging

### CloudWatch (AWS)

```hcl
resource "aws_cloudwatch_log_group" "eks" {
  name              = "/aws/eks/naar-noor"
  retention_in_days = 7
}
```

### Application Insights (Azure)

```hcl
resource "azurerm_application_insights" "main" {
  name                = "naar-noor-insights"
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
}
```

### Cloud Logging (GCP)

```hcl
resource "google_logging_project_sink" "kubernetes" {
  name        = "kubernetes-sink"
  destination = "storage.googleapis.com/${google_storage_bucket.logs.name}"
}
```

## Cost Optimization

- Dev environment: Minimal resources, 1 node
- Staging environment: 2 nodes, smaller instances
- Production environment: 3+ nodes, auto-scaling
- Reserved instances for predictable workloads
- Spot instances for batch jobs

## Troubleshooting

### State Lock

```bash
# Check lock
terraform state show

# Force unlock (if stuck)
terraform force-unlock <LOCK_ID>
```

### Provider Issues

```bash
# Update providers
terraform init -upgrade

# Check provider versions
terraform providers

# Debug mode
TF_LOG=DEBUG terraform apply
```

### Kubernetes Connection

```bash
# Update kubeconfig
aws eks update-kubeconfig --name naar-noor --region us-east-1
# or
az aks get-credentials --resource-group rg --name cluster
# or
gcloud container clusters get-credentials cluster --zone us-central1-a
```

## Checklist for Production

- [ ] Terraform state stored securely (S3 with encryption, state lock)
- [ ] Sensitive variables not committed to Git
- [ ] Multi-region/multi-AZ enabled
- [ ] Backup and disaster recovery configured
- [ ] Monitoring and alerting enabled
- [ ] IAM roles and policies reviewed
- [ ] Network security groups configured
- [ ] TLS certificates provisioned
- [ ] Load balancing configured
- [ ] Auto-scaling policies defined
- [ ] Cost estimation reviewed
- [ ] Disaster recovery tested

## Documentation

- [Terraform AWS Provider Docs](https://registry.terraform.io/providers/hashicorp/aws/latest/docs)
- [Terraform Azure Provider Docs](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- [Terraform GCP Provider Docs](https://registry.terraform.io/providers/hashicorp/google/latest/docs)
- [Terraform Best Practices](https://www.terraform.io/docs/cloud/guides/recommended-practices)

---

**Status**: Production Ready  
**Last Updated**: July 5, 2026  
**Maintained By**: DevOps Team
