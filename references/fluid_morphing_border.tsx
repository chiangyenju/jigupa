import React, { useEffect, useState } from 'react';

const MorphingGradientBorder = () => {
  const [animation, setAnimation] = useState(0);
  const [hoverScale, setHoverScale] = useState(1);

  useEffect(() => {
    const animate = () => {
      setAnimation(prev => (prev + 0.008) % (Math.PI * 2));
      requestAnimationFrame(animate);
    };
    animate();
  }, []);

  // Enhanced noise function for dramatic but fluid warping
  const noise = (x, y, t) => {
    const n1 = Math.sin(x * 0.1 + t) * Math.cos(y * 0.1 + t * 0.7);
    const n2 = Math.sin(x * 0.2 + t * 1.3) * Math.cos(y * 0.15 + t * 0.9);
    const n3 = Math.sin((x + y) * 0.05 + t * 1.6) * 0.8;
    return (n1 + n2 * 0.8 + n3) / 2; // Strong but smooth
  };

  // Dynamic gradient colors that morph
  const getGradientColors = (offset) => {
    const r1 = Math.sin(animation + offset) * 127 + 128;
    const g1 = Math.sin(animation + offset + Math.PI/3) * 127 + 128;
    const b1 = Math.sin(animation + offset + 2*Math.PI/3) * 127 + 128;
    
    const r2 = Math.sin(animation + offset + Math.PI) * 127 + 128;
    const g2 = Math.sin(animation + offset + Math.PI + Math.PI/3) * 127 + 128;
    const b2 = Math.sin(animation + offset + Math.PI + 2*Math.PI/3) * 127 + 128;
    
    return [
      `rgb(${Math.floor(r1)}, ${Math.floor(g1)}, ${Math.floor(b1)})`,
      `rgb(${Math.floor(r2)}, ${Math.floor(g2)}, ${Math.floor(b2)})`
    ];
  };

  // Dramatic but fluid warping border radius
  const getWarpingBorderRadius = (multiplier = 1) => {
    const baseRadius = 20;
    const warp1 = noise(0, 0, animation * 2.2) * 15 * multiplier;
    const warp2 = noise(50, 0, animation * 1.8) * 20 * multiplier;
    const warp3 = noise(100, 50, animation * 2.5) * 12 * multiplier;
    const warp4 = noise(0, 100, animation * 2.0) * 18 * multiplier;
    
    return `${baseRadius + warp1}px ${baseRadius + warp2}px ${baseRadius + warp3}px ${baseRadius + warp4}px`;
  };

  // Enhanced organic clip-path with dramatic but smooth warping
  const getWarpingClipPath = (intensity = 1, timeOffset = 0) => {
    const points = [];
    const numPoints = 14; // Good balance for smooth but visible warping
    
    for (let i = 0; i < numPoints; i++) {
      const angle = (i / numPoints) * Math.PI * 2;
      const baseRadius = 50;
      
      // Strong warping with multiple noise layers
      const warp1 = noise(Math.cos(angle) * 100, Math.sin(angle) * 100, animation * 2.2 + timeOffset) * 12 * intensity;
      const warp2 = noise(Math.cos(angle) * 50, Math.sin(angle) * 50, animation * 3.5 + timeOffset) * 6 * intensity;
      const warp3 = noise(Math.cos(angle) * 200, Math.sin(angle) * 200, animation * 1.8 + timeOffset) * 18 * intensity;
      
      const radius = baseRadius + warp1 + warp2 + warp3;
      const x = 50 + Math.cos(angle) * radius;
      const y = 50 + Math.sin(angle) * radius;
      
      points.push(`${x}% ${y}%`);
    }
    
    return `polygon(${points.join(', ')})`;
  };

  // Seamless rotating gradient angle
  const getGradientAngle = () => {
    return (animation * 57.2958) % 360;
  };

  const [color1, color2] = getGradientColors(0);
  const [color3, color4] = getGradientColors(Math.PI);

  const hoverIntensity = hoverScale > 1 ? 1.5 : 1;

  return (
    <div className="min-h-screen bg-gray-900 flex items-center justify-center p-8">
      <div className="relative">
        {/* Dramatic warping gradient light source layers - visible breathing effect */}
        {[...Array(5)].map((_, layerIndex) => {
          const timeOffset = layerIndex * Math.PI / 2.5;
          const warpIntensity = (0.9 + layerIndex * 0.4) * hoverIntensity;
          const [layerColor1, layerColor2] = getGradientColors(layerIndex * 0.4);
          
          return (
            <div
              key={`warp-layer-${layerIndex}`}
              className="absolute inset-0 pointer-events-none"
              style={{
                background: `linear-gradient(${getGradientAngle() + layerIndex * 30}deg, ${layerColor1}, ${layerColor2}, transparent)`,
                borderRadius: getWarpingBorderRadius(warpIntensity),
                clipPath: getWarpingClipPath(warpIntensity, timeOffset),
                filter: `blur(${4 + layerIndex * 3}px)`, // Moderate blur to keep warping visible
                opacity: 0.7 - layerIndex * 0.1,
                transform: `scale(${1.03 + layerIndex * 0.04}) rotate(${animation * (6 + layerIndex * 2)}deg)`,
                width: '330px',
                height: '250px',
                left: '-5px',
                top: '-5px',
              }}
            />
          );
        })}

        {/* Audio-reactive style morphing containers with dramatic warping */}
        {[...Array(3)].map((_, i) => {
          const warpPattern = i + 1;
          const intensity = Math.abs(Math.sin(animation * (1.8 + i * 0.5))) * 0.8 + 0.2;
          const warpIntensity = intensity * hoverIntensity;
          
          return (
            <div
              key={`morph-container-${i}`}
              className="absolute inset-0 pointer-events-none"
              style={{
                background: `conic-gradient(from ${getGradientAngle() + i * 120}deg, ${getGradientColors(i * 0.6)[0]}, ${getGradientColors(i * 0.6 + Math.PI)[1]}, ${getGradientColors(i * 0.6)[0]})`,
                borderRadius: getWarpingBorderRadius(warpIntensity * warpPattern),
                clipPath: getWarpingClipPath(warpIntensity * warpPattern, i * Math.PI / 1.5),
                filter: `blur(${6 + i * 4}px)`, // Visible warping
                opacity: intensity * 0.5,
                transform: `scale(${1.08 + intensity * 0.15}) rotate(${animation * (12 - i * 3)}deg)`,
                width: '340px',
                height: '260px',
                left: '-10px',
                top: '-10px',
              }}
            />
          );
        })}

        {/* Stable outer border container - no warping */}
        <div 
          className="relative p-1 transition-transform duration-300 ease-out z-10"
          style={{
            background: `linear-gradient(${getGradientAngle()}deg, ${color1}, ${color2}, ${color3}, ${color4})`,
            borderRadius: '20px',
            transform: `scale(${hoverScale})`,
            filter: 'blur(0.5px)',
          }}
          onMouseEnter={() => setHoverScale(1.05)}
          onMouseLeave={() => setHoverScale(1)}
        >
          {/* Stable inner glow effect - no warping */}
          <div 
            className="absolute inset-0 opacity-60"
            style={{
              background: `linear-gradient(${getGradientAngle() + 180}deg, ${color2}, ${color1})`,
              borderRadius: '20px',
              filter: 'blur(8px)',
              transform: 'scale(1.1)',
            }}
          />
          
          {/* Content container with stable shape */}
          <div 
            className="relative bg-gray-800"
            style={{
              borderRadius: '20px',
              width: '320px',
              height: '240px',
            }}
          >
          </div>
        </div>

        {/* Audio spectrum-style warping effects with visible dramatic movement */}
        {[...Array(12)].map((_, i) => {
          const angle = (i / 12) * Math.PI * 2;
          const intensity = Math.abs(Math.sin(animation * 3.5 + i * 0.5)) * 0.9 + 0.1;
          const warpIntensity = intensity * hoverIntensity;
          
          // Dynamic positioning with strong warping for audio-reactive feel
          const baseX = 50 + Math.cos(angle) * (25 + intensity * 20);
          const baseY = 50 + Math.sin(angle) * (25 + intensity * 20);
          const warpX = noise(i * 20, 0, animation * 2.8) * 20 * warpIntensity;
          const warpY = noise(0, i * 20, animation * 3.2) * 20 * warpIntensity;
          
          const x = baseX + warpX;
          const y = baseY + warpY;
          
          return (
            <div
              key={i}
              className="absolute inset-0 pointer-events-none"
              style={{
                background: `radial-gradient(ellipse ${70 + intensity * 50}% ${50 + intensity * 70}% at ${x}% ${y}%, ${getGradientColors(i * 0.2)[0]}${Math.floor(intensity * 50)}, transparent 70%)`,
                borderRadius: getWarpingBorderRadius(warpIntensity),
                filter: `blur(${8 + intensity * 8}px)`, // Balanced blur - visible but smooth
                opacity: intensity * 0.4,
                transform: `scale(${1.15 + intensity * 0.4}) rotate(${animation * 12 + i * 30}deg)`,
                clipPath: getWarpingClipPath(warpIntensity * 0.9, i * 0.5),
              }}
            />
          );
        })}
        
        {/* Ultra-fluid secondary spectrum layer */}
        {[...Array(6)].map((_, i) => {
          const angle = (i / 6) * Math.PI * 2 + animation * 0.3;
          const intensity = Math.abs(Math.sin(animation * 2 + i * 0.6)) * 0.5 + 0.4;
          const warpIntensity = intensity * hoverIntensity * 0.5;
          
          // Gentler warping pattern
          const warpX = noise(i * 25, 0, animation * 2.5) * 18 * warpIntensity;
          const warpY = noise(0, i * 25, animation * 1.8) * 18 * warpIntensity;
          const distance = 20 + intensity * 15 + Math.sin(animation * 3 + i) * 8;
          
          const x = Math.max(15, Math.min(85, 50 + Math.cos(angle) * distance + warpX));
          const y = Math.max(15, Math.min(85, 50 + Math.sin(angle) * distance + warpY));
          
          return (
            <div
              key={`second-${i}`}
              className="absolute inset-0 pointer-events-none"
              style={{
                background: `radial-gradient(ellipse ${60 + intensity * 100}% ${45 + intensity * 70}% at ${x}% ${y}%, ${getGradientColors(i * 0.25 + Math.PI)[1]}20, transparent 55%)`,
                borderRadius: getWarpingBorderRadius(warpIntensity),
                filter: `blur(${25 + intensity * 15}px)`, // Super blur
                opacity: intensity * 0.15,
                transform: `scale(${1.3 + intensity * 0.3}) rotate(${animation * 12 + i * 60}deg)`,
                clipPath: getWarpingClipPath(warpIntensity, i * 0.6 + Math.PI),
              }}
            />
          );
        })}

        {/* Ultra-soft organic outer warping layers */}
        {[...Array(8)].map((_, i) => {
          const intensity = Math.abs(Math.sin(animation * 1.5 + i)) * 0.6 + 0.4;
          const warpIntensity = intensity * hoverIntensity * 0.4;
          
          return (
            <div
              key={`warp-${i}`}
              className="absolute inset-0 pointer-events-none"
              style={{
                background: `radial-gradient(ellipse 150% 120% at center, ${getGradientColors(i * 0.3)[0]}10, transparent 50%)`,
                borderRadius: getWarpingBorderRadius(warpIntensity * 2),
                filter: `blur(${35 + i * 12}px)`, // Massive blur for ultra-soft effect
                opacity: intensity * 0.1,
                transform: `scale(${1.4 + intensity * 0.5 + i * 0.15}) rotate(${animation * (5 + i)}deg)`,
                clipPath: getWarpingClipPath(warpIntensity * 1.5, i * Math.PI / 4),
              }}
            />
          );
        })}
        
        {/* Ultra-soft base ambient glow */}
        <div 
          className="absolute inset-0 pointer-events-none opacity-08"
          style={{
            background: `radial-gradient(ellipse 200% 150% at center, ${getGradientColors(0)[0]}15, transparent 60%)`,
            borderRadius: getWarpingBorderRadius(hoverIntensity * 0.3),
            filter: 'blur(50px)', // Maximum softness
            transform: `scale(${1.6 + Math.sin(animation * 0.6) * 0.2})`,
            clipPath: getWarpingClipPath(0.4 * hoverIntensity, animation * 0.2),
          }}
        />
      </div>
    </div>
  );
};

export default MorphingGradientBorder;