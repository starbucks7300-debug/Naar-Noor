# DevOps Folder Organization Guide

This document explains the new DevOps folder structure and how to work with infrastructure-as-code for Naar-Noor.

## New Structure

```
Naar-Noor/
├── devops/                          # NEW: Centralized DevOps folder
│   ├── README.md                    # Main DevOps overview
│   ├── ORGANIZATION.md              # This file - folder structure guide
│   │
│   ├── kubernetes/                  # K8s manifests and Helm charts
│   │   ├── README.md
│   │   ├── base/                    # Base manifests from /k8s
│   │   │   ├── namespace.yaml
│   │   │   ├── backend-deployment.yaml
│   │   │   ├── frontend-deployment.yaml
│   │   │   ├── database-statefulset.yaml
│   │   │   ├── services.yaml
│   │   │   ├── ingress.yaml
│   │   │   ├── hpa.yaml
│   │   │   ├── secrets.yaml
│   │   │   ├── service-accounts.yaml
│   │   │   └── rbac.yaml
│   │   ├── overlays/                # Environment-specific patches
│   │   │   ├── dev/
│   │   │   ├── staging/
│   │   │   └── production/
│   │   └── helm/                    # Helm charts for packaging
│   │       └── naar-noor/
│   │
│   ├── terraform/                   # Infrastructure-as-code from /terraform
│   │   ├── README.md
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── outputs.tf
│   │   ├── providers.tf
│   │   ├── backends.tf
│   │   ├── locals.tf
│   │   ├── environments/            # Environment-specific variables
│   │   │   ├── dev.tfvars
│   │   │   ├── staging.tfvars
│   │   │   └── production.tfvars
│   │   ├── modules/                 # Reusable Terraform modules
│   │   │   ├── kubernetes/
│   │   │   ├── database/
│   │   │   ├── networking/
│   │   │   └── storage/
│   │   ├── aws/                     # AWS-specific resources
│   │   ├── azure/                   # Azure-specific resources
│   │   └── gcp/                     # GCP-specific resources
│   │
│   ├── docker/                      # Docker configurations
│   │   ├── backend/
│   │   │   └── Dockerfile           # Backend ASP.NET Core image
│   │   ├── frontend/
│   │   │   └── Dockerfile           # Frontend Angular image
│   │   └── scripts/                 # Docker helper scripts
│   │
│   ├── ci-cd/                       # GitHub Actions workflows
│   │   ├── build.yml
│   │   ├── deploy.yml
│   │   ├── test.yml
│   │   └── security.yml
│   │
│   ├── monitoring/                  # Prometheus & Grafana config
│   │   ├── prometheus.yml
│   │   ├── grafana.yml
│   │   └── dashboards/
│   │
│   ├── scripts/                     # Deployment helper scripts
│   │   ├── setup.sh                 # Initial cluster setup
│   │   ├── deploy.sh                # Deployment automation
│   │   ├── backup.sh                # Database backups
│   │   └── rollback.sh              # Deployment rollback
│   │
│   └── docs/                        # DevOps documentation
│       ├── DEPLOYMENT.md
│       ├── KUBERNETES.md
│       ├── TERRAFORM.md
│       └── TROUBLESHOOTING.md
│
├── api-server/                      # Backend (unchanged)
├── naar-noor/                       # Frontend (unchanged)
├── mobile/                          # Mobile app (unchanged)
├── desktop/                         # Desktop app (unchanged)
├── docs/                            # Project documentation
├── docker-compose.yml               # Production compose (unchanged)
├── docker-compose.dev.yml           # Development compose (unchanged)
└── Dockerfile                       # Removed - use devops/docker/
```

## Migration Path

### What Moved

| From | To | Status |
|------|-----|--------|
| `/k8s/*` | `/devops/kubernetes/base/` | To move |
| `/terraform/*` | `/devops/terraform/` | To move |
| `/docker/backend` | `/devops/docker/backend/` | To move |
| `/docker/frontend` | `/devops/docker/frontend/` | To move |
| `/.github/workflows/` | `/devops/ci-cd/` | Reference only |

### What Stays

| Path | Reason |
|------|--------|
| `/docker-compose.yml` | Root-level for easy discovery |
| `/docker-compose.dev.yml` | Root-level for easy discovery |
| `/.github/workflows/` | GitHub Actions requirement |
| `/docs/` | Project documentation (not DevOps-specific) |

## File References After Move

### Terraform References

After moving k8s and terraform to devops folder, update file references:

**Before:**
```hcl
manifest = yamldecode(file("${path.module}/../k8s/backend-deployment.yaml"))
```

**After:**
```hcl
manifest = yamldecode(file("${path.module}/../kubernetes/base/backend-deployment.yaml"))
```

### GitHub Actions References

If workflows reference k8s files:

**Before:**
```yaml
- run: kubectl apply -f k8s/
```

**After:**
```yaml
- run: kubectl apply -f devops/kubernetes/base/
```

### Deployment Script References

**Before:**
```bash
kubectl apply -f terraform/k8s/*.yaml
```

**After:**
```bash
kubectl apply -f devops/kubernetes/base/
```

## How to Use

### Development Setup

```bash
cd Naar-Noor/devops/kubernetes

# Deploy locally
kubectl apply -k overlays/dev/
```

### Staging Deployment

```bash
cd Naar-Noor/devops/terraform

# Plan infrastructure changes
terraform plan -var-file="environments/staging.tfvars"

# Apply infrastructure
terraform apply -var-file="environments/staging.tfvars"

# Deploy applications
kubectl apply -k ../kubernetes/overlays/staging/
```

### Production Deployment

```bash
cd Naar-Noor/devops

# 1. Plan infrastructure
cd terraform
terraform plan -var-file="environments/production.tfvars" -out=tfplan

# 2. Review and apply
terraform apply tfplan

# 3. Deploy applications
cd ../kubernetes
kubectl apply -k overlays/production/
```

## Benefits of New Organization

✅ **Centralized DevOps**
- All infrastructure code in one folder
- Easier to navigate and maintain
- Clear separation from application code

✅ **Scalability**
- Room for multiple cloud providers
- Environment-specific configurations
- Reusable modules

✅ **Team Collaboration**
- DevOps team has dedicated folder
- Clear responsibilities
- Easier onboarding

✅ **CI/CD Integration**
- All automation tools referenced from one place
- Consistent deployment process
- Audit trail preserved in Git

## Step-by-Step Migration

### Phase 1: Create devops Structure (DONE ✅)
- [ ] Create devops/ folder
- [ ] Create README.md documentation
- [ ] Create kubernetes/README.md
- [ ] Create terraform/README.md
- [ ] Create all subdirectories

### Phase 2: Copy Files (TO DO)
```bash
# Copy k8s files
cp -r k8s/* devops/kubernetes/base/

# Copy terraform files (excluding k8s references)
cp -r terraform/* devops/terraform/

# Update Terraform references in devops/terraform/main.tf
```

### Phase 3: Update References (TO DO)
- [ ] Update Terraform file paths
- [ ] Update GitHub Actions workflows
- [ ] Update deployment scripts
- [ ] Update documentation links

### Phase 4: Testing (TO DO)
- [ ] Test local k8s deployment
- [ ] Test Terraform plan/apply
- [ ] Test CI/CD workflows
- [ ] Test production deployment simulation

### Phase 5: Cleanup (TO DO)
- [ ] Archive old /k8s folder (don't delete)
- [ ] Archive old /terraform folder (don't delete)
- [ ] Update .gitignore if needed
- [ ] Update README.md links

## File Naming Conventions

```
devops/
├── <component>-<type>.<ext>          # Individual files
│   ├── backend-deployment.yaml
│   ├── frontend-service.yaml
│   └── database-statefulset.yaml
│
├── <environment>-<component>.<ext>   # Environment-specific files
│   ├── dev-backend.tfvars
│   ├── prod-backend.tfvars
│   └── staging-frontend.tfvars
│
└── Makefile                           # Automation commands
```

## Documentation Structure

```
devops/docs/
├── DEPLOYMENT.md          # How to deploy all components
├── KUBERNETES.md          # K8s-specific guide
├── TERRAFORM.md           # Terraform-specific guide
├── TROUBLESHOOTING.md     # Common issues and solutions
└── CLOUD_SETUP.md         # AWS/Azure/GCP setup guides
```

## Environment Configuration

### Development
- Local Kubernetes (Docker Desktop / Minikube)
- PostgreSQL in Docker
- No cloud resources

### Staging
- Managed Kubernetes (EKS / AKS / GKE)
- Cloud database (RDS / Azure DB / Cloud SQL)
- Reduced replicas
- Staging domain

### Production
- Managed Kubernetes with HA
- Multi-region capability
- Full monitoring and logging
- Production domain with SSL
- Backup and DR enabled

## Monitoring & Alerts

All monitoring configurations centralized in `/devops/monitoring/`:
- Prometheus scrape configs
- Grafana dashboard definitions
- Alert rules
- Custom metrics

## Scripts & Automation

All automation scripts in `/devops/scripts/`:
- `setup.sh` - First-time cluster setup
- `deploy.sh` - Deployment automation
- `backup.sh` - Database backup
- `rollback.sh` - Deployment rollback
- `health-check.sh` - Infrastructure health checks

## Security Considerations

⚠️ **Never commit to Git:**
- Secrets (passwords, API keys, tokens)
- Private SSH keys
- SSL certificates
- terraform.tfstate files

✅ **Always use:**
- `.gitignore` for sensitive files
- Secret management tools (AWS Secrets Manager, Azure Key Vault)
- Encrypted environment variables in CI/CD
- RBAC for Kubernetes access

## Rollback Procedure

In case of deployment issues:

```bash
cd devops/kubernetes
kubectl rollout undo deployment/naar-noor-backend -n naar-noor
kubectl rollout undo deployment/naar-noor-frontend -n naar-noor

# Or use rollback script
../scripts/rollback.sh production
```

## Cost Management

All cloud resources managed through Terraform:
- Easy to destroy unused environments
- Cost tracking by environment
- Resource optimization defined in code
- Spot instances for non-critical workloads

## Next Steps

1. ✅ Create folder structure
2. ⬜ Copy k8s and terraform files
3. ⬜ Update file references
4. ⬜ Test all deployments
5. ⬜ Update CI/CD workflows
6. ⬜ Document any custom configurations
7. ⬜ Archive old folders
8. ⬜ Deploy to production

---

**Status**: Folder structure created, ready for file migration  
**Last Updated**: July 5, 2026  
**Next**: Move k8s and terraform files to devops/
