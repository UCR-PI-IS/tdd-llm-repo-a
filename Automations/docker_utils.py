"""Shared utilities for Docker build, test, and metrics scripts."""

import os
import subprocess
import sys
from datetime import datetime
from pathlib import Path


COLORS = {
    "RED": "\033[0;31m",
    "GREEN": "\033[0;32m",
    "YELLOW": "\033[1;33m",
    "NC": "\033[0m",
}


def cprint(msg: str, color: str) -> None:
    print(f"{COLORS.get(color, '')}{msg}{COLORS['NC']}")


def generate_timestamp() -> str:
    return datetime.now().strftime("%Y-%m-%d_%H-%M-%S")


def get_workspace_root() -> Path:
    return Path(__file__).resolve().parent.parent


def print_banner(title: str, timestamp: str, output_dir: str) -> None:
    cprint(f"=== {title} ===", "YELLOW")
    print(f"Timestamp: {timestamp}")
    print(f"Output directory: {output_dir}")
    print()


def print_result(success: bool, output_dir: str, log_file: str = "") -> None:
    print()
    if success:
        cprint(f"\u2713 Succeeded", "GREEN")
        cprint(f"Results: {output_dir}/", "GREEN")
    else:
        cprint(f"\u2717 Failed", "RED")
        if log_file:
            cprint(f"Logs: {output_dir}/{log_file}", "RED")
    print()
    print(f"Output location: {output_dir}")


def build_docker_image(image_name: str, dockerfile: str) -> None:
    cprint("Building Docker image...", "YELLOW")
    subprocess.run(
        ["docker", "build", "-t", image_name, "-f", dockerfile, "."],
        check=True,
        cwd=get_workspace_root(),
    )


def write_inner_script(output_dir: Path, filename: str, content: str) -> Path:
    script_path = output_dir / filename
    script_path.write_text(content)
    os.chmod(script_path, 0o755)
    return script_path


def run_docker_container(
    image_name: str,
    workspace_root: Path,
    output_dir: Path,
    inner_script: str,
    log_file: str,
    env_vars: dict[str, str] | None = None,
) -> int:
    cmd = [
        "docker", "run", "--rm",
        "-v", f"{workspace_root}:/workspace",
        "-v", f"{workspace_root / output_dir}:/output",
        "-v", f"{Path.home() / '.nuget/packages'}:/root/.nuget/packages",
        "-w", "/workspace",
    ]
    if env_vars:
        for k, v in env_vars.items():
            cmd.extend(["-e", f"{k}={v}"])
    cmd.extend([image_name, "bash", inner_script])

    log_path = workspace_root / output_dir / log_file
    try:
        with open(log_path, "w") as log_f:
            proc = subprocess.Popen(
                cmd,
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                cwd=workspace_root,
            )
            for line in iter(proc.stdout.readline, b""):
                text = line.decode("utf-8", errors="replace")
                sys.stdout.write(text)
                log_f.write(text)
            proc.wait()
        return proc.returncode
    except KeyboardInterrupt:
        proc.terminate()
        proc.wait()
        raise
