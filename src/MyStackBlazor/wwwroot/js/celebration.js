// ----------------------------------------------------------------
//  Celebration — lightweight canvas confetti. Draws on a temporary
//  full-screen, click-through canvas that removes itself when done.
// ----------------------------------------------------------------

const DEFAULT_COLORS = ['#f43f5e', '#f59e0b', '#10b981', '#3b82f6', '#8b5cf6', '#ec4899', '#facc15'];

export function burst(options) {
    const o = options || {};
    const reduce = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (reduce) return Promise.resolve();

    const count = Math.max(1, Math.min(o.particleCount || 160, 600));
    const duration = Math.max(500, o.durationMs || 3000);
    const colors = (o.colors && o.colors.length) ? o.colors : DEFAULT_COLORS;
    const originX = typeof o.originX === 'number' ? o.originX : 0.5;
    const originY = typeof o.originY === 'number' ? o.originY : 0.6;

    const dpr = window.devicePixelRatio || 1;
    const canvas = document.createElement('canvas');
    canvas.setAttribute('aria-hidden', 'true');
    Object.assign(canvas.style, {
        position: 'fixed', inset: '0', width: '100%', height: '100%',
        pointerEvents: 'none', zIndex: String(o.zIndex || 9999),
    });
    document.body.appendChild(canvas);
    const ctx = canvas.getContext('2d');

    const resize = () => {
        canvas.width = Math.floor(window.innerWidth * dpr);
        canvas.height = Math.floor(window.innerHeight * dpr);
    };
    resize();
    window.addEventListener('resize', resize);

    const w = () => canvas.width;
    const h = () => canvas.height;
    const particles = [];
    for (let i = 0; i < count; i++) {
        const angle = (-Math.PI / 2) + (Math.random() - 0.5) * (Math.PI * 0.9);
        const speed = (8 + Math.random() * 10) * dpr;
        particles.push({
            x: w() * originX,
            y: h() * originY,
            vx: Math.cos(angle) * speed,
            vy: Math.sin(angle) * speed,
            size: (5 + Math.random() * 6) * dpr,
            rot: Math.random() * Math.PI,
            vr: (Math.random() - 0.5) * 0.3,
            tilt: Math.random() * Math.PI,
            color: colors[i % colors.length],
            shape: Math.random() < 0.3 ? 'circle' : 'rect',
        });
    }

    const gravity = 0.35 * dpr;
    const drag = 0.985;
    const start = performance.now();

    return new Promise(resolve => {
        const frame = now => {
            const elapsed = now - start;
            const fade = elapsed > duration * 0.7 ? Math.max(0, 1 - (elapsed - duration * 0.7) / (duration * 0.3)) : 1;
            ctx.clearRect(0, 0, w(), h());
            ctx.globalAlpha = fade;
            for (const p of particles) {
                p.vx *= drag;
                p.vy = p.vy * drag + gravity;
                p.x += p.vx;
                p.y += p.vy;
                p.rot += p.vr;
                p.tilt += 0.1;
                ctx.save();
                ctx.translate(p.x, p.y);
                ctx.rotate(p.rot);
                ctx.fillStyle = p.color;
                if (p.shape === 'circle') {
                    ctx.beginPath();
                    ctx.arc(0, 0, p.size / 2, 0, Math.PI * 2);
                    ctx.fill();
                } else {
                    ctx.fillRect(-p.size / 2, -p.size / 4, p.size, (p.size / 2) * Math.abs(Math.cos(p.tilt)) + 1);
                }
                ctx.restore();
            }
            if (elapsed < duration) {
                requestAnimationFrame(frame);
            } else {
                window.removeEventListener('resize', resize);
                canvas.remove();
                resolve();
            }
        };
        requestAnimationFrame(frame);
    });
}
