# DevOps - Infrastructure as Code

This directory contains all infrastructure-as-code (IaC) and Kubernetes orchestration configurations for Naar-Noor.

## Structure

```
devops/
├── kubernetes/          # Kubernetes manifests and Helm charts
│   ├── base/           # Base manifests (development)
│   ├── overlays/       # Kustomize overlays (staging, production)
│   ├── helm/           # Helm charts for packaging
│   └── README.md
├── terraform/          # Terraform IaC for cloud infrastructure
│   ├── aws/            # AWS-specific resources
│   ├── azure/          # Azure-specific resources
│   ├── gcp/            # GCP-specific resources
│   ├── modules/        # Reusable Terraform modules
│   ├── environments/   # Environment-specific configurations
│   └── README.md
├── docker/             # Docker build contexts and scripts
│   ├── backend/        # Backend Dockerfile
│   ├── frontend/       # Frontend Dockerfile
│   └── scripts/        # Docker helper scripts
├── ci-cd/              # GitHub Actions workflows
│   ├── build.yml
│   ├── deploy.yml
│   └── test.yml
├── monitoring/         # Prometheus, Grafana configurations
│   ├── prometheus.yml
│   ├── grafana.yml
│   └── dashboards/
├── docs/               # DevOps documentation
│   ├── DEPLOYMENT.md
│   ├── KUBERNETES.md
│   ├── TERRAFORM.md
│   └── TROUBLESHOOTING.md
└── scripts/            # Helper scripts for deployment
    ├── setup.sh        # Initial setup
    ├── deploy.sh       # Deployment automation
    ├── backup.sh       # Database backups
    └── rollback.sh     # Deployment rollback
```

## Quick Start

### Prerequisites
- Docker & Docker Compose
- kubectl (Kubernetes client)
- terraform (for IaC)
- helm (for package management)

### Development Environment

```bash
cd devops

# Start local Kubernetes (minikube or Docker Desktop)
minikube start
# or enable Kubernetes in Docker Desktop settings

# Deploy to local cluster
kubectl apply -f kubernetes/base/namespace.yaml
kubectl apply -f kubernetes/base/

# View status
kubectl get all -n naar-noor
```

### Production Deployment

```bash
# With Terraform
cd terraform
terraform init
terraform plan
terraform apply

# Or with kubectl directly
kubectl apply -f kubernetes/overlays/production/

# Verify deployment
kubectl get deployments -n naar-noor
kubectl get services -n naar-noor
```

## Components

### Kubernetes (`kubernetes/`)
- **Namespaces**: Logical separation (development, staging, production)
- **Deployments**: Backend API, Frontend web server
- **StatefulSets**: Database (PostgreSQL with persistence)
- **Services**: Internal and external networking
- **Ingress**: HTTP/HTTPS routing
- **HPA**: Horizontal Pod Autoscaling
- **ConfigMaps**: Configuration management
- **Secrets**: Sensitive data (passwords, keys)
- **RBAC**: Service accounts and role bindings

### Terraform (`terraform/`)
- **Providers**: AWS, Azure, GCP support
- **Networking**: VPCs, subnets, security groups
- **Compute**: EC2, App Service, Compute Engine
- **Storage**: S3, Blob, Cloud Storage
- **Database**: RDS, Azure Database, Cloud SQL
- **Kubernetes**: EKS, AKS, GKE cluster setup
- **Modules**: Reusable infrastructure components

### Docker (`docker/`)
- **Backend**: Multi-stage build for .NET Core
- **Frontend**: Node.js build + Nginx serve
- **Optimization**: Minimal base images, layer caching

### Monitoring (`monitoring/`)
- **Prometheus**: Metrics collection
- **Grafana**: Visualization dashboards
- **Alerts**: Infrastructure monitoring alerts

### CI/CD (`ci-cd/`)
- **GitHub Actions**: Automated builds, tests, deployments
- **Build pipeline**: Docker image creation
- **Test pipeline**: Automated testing
- **Deploy pipeline**: Push to registries and clusters

## Environment Configuration

### Development
- Local Kubernetes cluster
- Docker Compose for local development
- No cloud resources required

### Staging
- Managed Kubernetes cluster (EKS/AKS/GKE)
- Cloud database (RDS/Azure Database/Cloud SQL)
- Reduced replicas and resource limits
- Staging domain and SSL certificates

### Production
- High-availability Kubernetes cluster
- Multi-region deployment
- Auto-scaling enabled
- Full monitoring and logging
- Backup and disaster recovery
- Production domain and SSL certificates

## Deployment Workflow

```
Git Push → GitHub Actions
    ↓
Run Tests & Build Docker Images
    ↓
Push to Container Registry (GHCR/ECR/ACR)
    ↓
Update Kubernetes Deployment
    ↓
Rolling Update / Blue-Green Deployment
    ↓
Health Checks & Monitoring
    ↓
Rollback (if failed)
```

## Common Commands

### Kubernetes

```bash
# View resources
kubectl get all -n naar-noor
kubectl describe pod <pod-name> -n naar-noor
kubectl logs <pod-name> -n naar-noor

# Execute commands in pod
kubectl exec -it <pod-name> -n naar-noor -- /bin/bash

# Port forwarding
kubectl port-forward <pod-name> 8080:8080 -n naar-noor

# Apply changes
kubectl apply -f kubernetes/base/

# Rollback
kubectl rollout undo deployment/naar-noor-backend -n naar-noor

# Scale deployment
kubectl scale deployment naar-noor-backend --replicas=5 -n naar-noor
```

### Terraform

```bash
# Initialize (first time)
cd terraform
terraform init

# Plan changes
terraform plan

# Apply changes
terraform apply

# Destroy resources
terraform destroy

# View state
terraform state list
terraform state show <resource>
```

### Docker

```bash
# Build image
docker build -f docker/backend/Dockerfile -t naar-noor:backend .

# Run container
docker run -p 8080:80 naar-noor:backend

# Push to registry
docker push ghcr.io/username/naar-noor:backend:latest
```

## Security Best Practices

✅ **Implemented:**
- Pod security policies
- Network policies
- RBAC (Role-Based Access Control)
- Secrets encryption at rest
- Resource limits and quotas
- Non-root container users
- Read-only filesystems

✅ **Configured:**
- Health checks (liveness & readiness probes)
- Security contexts
- Pod anti-affinity (spread across nodes)
- Network segmentation

⚠️ **To Implement:**
- Kubernetes network policies
- Service mesh (Istio/Linkerd) - optional
- Pod security standards
- Image scanning and vulnerability checks
- Audit logging

## Monitoring & Logging

### Prometheus Metrics
- Pod CPU/memory usage
- Request latency
- Error rates
- Database connections
- Custom application metrics

### Grafana Dashboards
- Infrastructure overview
- Application performance
- Resource utilization
- Error tracking

### Logs
- Elasticsearch (optional)
- CloudWatch (AWS)
- Application Insights (Azure)
- Cloud Logging (GCP)

## Disaster Recovery

### Backups
- Daily database snapshots
- Infrastructure code in Git (immutable)
- Container images in registry

### Failover
- Multi-region deployment (optional)
- Automatic health check restarts
- Manual rollback capability

## Cost Optimization

- Resource requests/limits prevent over-provisioning
- Horizontal Pod Autoscaling (HPA) scales on demand
- Spot instances for non-critical workloads (optional)
- Reserved instances for baseline load

## Troubleshooting

See `docs/TROUBLESHOOTING.md` for common issues:
- Pod not starting
- Deployment rollout stuck
- Performance issues
- Database connectivity

## Support & Documentation

- **Kubernetes Guide**: `docs/KUBERNETES.md`
- **Terraform Guide**: `docs/TERRAFORM.md`
- **Deployment**: `docs/DEPLOYMENT.md`
- **Troubleshooting**: `docs/TROUBLESHOOTING.md`

## Version History

- **v1.0** - Initial Kubernetes and Terraform setup
- **v1.1** - Added monitoring and auto-scaling
- **v1.2** - Multi-environment support (dev, staging, prod)

---

**Last Updated**: July 5, 2026  
**Maintained By**: DevOps Team  
**Status**: Production Ready
